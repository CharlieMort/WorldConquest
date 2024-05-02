using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceHandlerScript : MonoBehaviour
{
    public List<GameObject> dice; // Both attacker and defender dice - used for group handling
    bool thrown = false; // if the dice have been thrown
    bool moving = false; // if they are on their way to their target positions and rotations
    bool smashing = false; // are they smashing happens after results
    public GameObject attackerDie; // attackerDie prefab
    public GameObject defenderDie; // defenderDie prefab

    Vector3[] targetPosys; // Used to store target positions for die once they stopped moving
    List<Quaternion> targetRotys; // Used to store target rotations for die once they stopped moving
    int[] diceOutcomes; // the overall outcomes for each dice 
    int aDiceNum = -1; // the number of dice the attacker has chosen to roll

    // Animation variables
    public float preSmashDelay = 0.5f;
    public float postSmashDelay = 2f;
    public float smashSpeed = 50f;

    List<int> winners; // Die to Die comparison which one won

    List<GameObject> aDice = new List<GameObject>(); // attacker Dice
    List<GameObject> dDice = new List<GameObject>(); // defender Dice

    public Action<int[]> ActionAfterDiceRoll; // Action called once whole sequence has finished

    private void Start()
    {
        //ThrowDice(3, 2); // for testing reasons
    }

    // Called every frame
    private void Update()
    {
        if (thrown) // have the dice been thrown
        {
            if (moving) // are they on their way to the target
            {
                DiceMoving();
            }
            else // Dice are still rolling
            {
                bool stoppedRolling = true;
                for (int i = 0; i<dice.Count; i++)
                {
                    if (dice[i].transform.GetComponent<Rigidbody>().velocity.magnitude > 0.1f) // are each dice stood still
                    {
                        stoppedRolling = false;
                    }
                }
                if (stoppedRolling) // if all pass
                {
                    moving = true; // theyre now on their way to the targets
                    GetDiceRoll(); // create the target positions and rotations based on which side is up and sort them
                }
            }
        }
        else if (smashing)
        {
            DoTheSmashing(); // pretty self explanitory
        }
    }

    // Logic method to get the integer value for each die based on its Y position of each face
    // Called once
    private void GetDiceRoll()
    {
        targetPosys = new Vector3[dice.Count];
        targetRotys = new List<Quaternion>();
        diceOutcomes = new int[dice.Count];
        for (int i = 0; i < dice.Count; i++)
        {
            // Find which child gameObject of the dice has the largest Y
            // Each child is named after a number and if they're the highest that dice rolled that number
            Transform highest = dice[i].transform.GetChild(0);
            for (int j = 1; j < dice[i].transform.childCount; j++)
            {
                if (dice[i].transform.GetChild(j).position.y > highest.position.y)
                {
                    highest = dice[i].transform.GetChild(j);
                }
            }
            int rolledNum = int.Parse(highest.name);
            diceOutcomes[i] = rolledNum;
            dice[i].name = rolledNum.ToString();
            // Append the color or what side they're on (attacker or defender) used later for sorting
            if (i >= aDiceNum)
            {
                dice[i].name += " blue";
            }
            else
            {
                dice[i].name += " red";
            }
            dice[i].GetComponent<Rigidbody>().isKinematic = true; // stop physics + gravity
        }
        int[] aOut = new int[aDiceNum];
        int[] dOut = new int[dice.Count - aDiceNum];
        int aIdx = 0;
        int dIdx = 0;
        // cheeky little insertion sort based on the die's name
        for (int i = 0; i < dice.Count; i++)
        {
            int bigIdx = i;
            for (int k = i+1; k < dice.Count; k++)
            {
                if (int.Parse(dice[k].name.Split(" ")[0]) > int.Parse(dice[bigIdx].name.Split(" ")[0]))
                {
                    bigIdx = k;
                }
            }
            GameObject tmp = dice[i];
            dice[i] = dice[bigIdx];
            dice[bigIdx] = tmp;
            if (dice[i].name.Split(" ")[1] == "blue")
            {
                // Since insertion sort its sort in order so we can create the target positions as each dice is sorted
                targetPosys[i] = new Vector3(10f, 15f, 6f - (dIdx * 8f));
                dOut[dIdx] = int.Parse(dice[i].name.Split(" ")[0]);
                dIdx++;
                dDice.Add(dice[i]);
            }
            else
            {
                targetPosys[i] = new Vector3(-10f, 15f, 6f - (aIdx * 8f));
                aOut[aIdx] = int.Parse(dice[i].name.Split(" ")[0]);
                aIdx++;
                aDice.Add(dice[i]);
            }
            // Set the target rotation so that the dice roll is in the correct orientaion  
            switch (int.Parse(dice[i].name.Split(" ")[0]))
            {
                case 1:
                    targetRotys.Add(Quaternion.Euler(new Vector3(270f, 0f, 0f)));
                    break;
                case 2:
                    targetRotys.Add(Quaternion.Euler(new Vector3(0f, 0f, 270f)));
                    break;
                case 3:
                    targetRotys.Add(Quaternion.Euler(new Vector3(0f, 0f, 0f)));
                    break;
                case 4:
                    targetRotys.Add(Quaternion.Euler(new Vector3(180f, 0f, 0f)));
                    break;
                case 5:
                    targetRotys.Add(Quaternion.Euler(new Vector3(0f, 0f, 90f)));
                    break;
                case 6:
                    targetRotys.Add(Quaternion.Euler(new Vector3(90f, 0f, 0f)));
                    break;
            }
        }
    }

    private void DiceMoving()
    {
        bool finished = true;
        // For all dice
        for (int i = 0; i < dice.Count; i++)
        {
            // Lerp is meant to be linear but we use a changing T value so its non-linear - gives a smooth not snappy feel
            dice[i].transform.transform.position = Vector3.Lerp(dice[i].transform.transform.position, targetPosys[i], 2.5f * Time.deltaTime);
            dice[i].transform.transform.localRotation = Quaternion.Lerp(dice[i].transform.transform.localRotation, targetRotys[i], 2.5f * Time.deltaTime);
            float distance = (targetPosys[i] - dice[i].transform.transform.position).magnitude;
            float rDistance = Quaternion.Dot(dice[i].transform.transform.localRotation, targetRotys[i]);
            // Since Lerp not linear it wont reach its exact target for a weird amount of time so we check if its close enough then snap it to the target position
            if (distance < 0.5f && Mathf.Abs(rDistance) >= 0.99f)
            {
                dice[i].transform.transform.position = targetPosys[i];
                dice[i].transform.transform.localRotation = targetRotys[i];
            }
            else
            {
                finished = false;
            }
        }
        if (finished)
        {
            moving = false;
            thrown = false;
            thrown = false;
            // Delayed to allow the user to register what the rolls were
            // Changing the delay can be tricky as you might think its long enough were someone else might feel rushed
            // Want the whole thing to feel snappy but not rushed
            Invoke("Smash", preSmashDelay);
        }
    }


    // This method not only makes the dice "smash" but makes the output for the whole animation
    // It creates the winners array which contains which dice won and so the attack handler can add or subtract troops
    // This method is called once
    public void Smash()
    {
        smashing = true;
        winners = new List<int>();
        for (int i = 0; i<dDice.Count; i++)
        {
            int attackNum = int.Parse(aDice[i].name.Split(" ")[0]);
            int defendNum = int.Parse(dDice[i].name.Split(" ")[0]);
            if (attackNum > defendNum)
            {
                winners.Add(0);
                // We only want to create sub cubes for the losing dice which explodes as this is a very expensive operation
                dDice[i].GetComponent<Explode>().SpawnCubes();
            }
            else
            {
                winners.Add(1);
                aDice[i].GetComponent<Explode>().SpawnCubes();
            }
            // Disabled colliders to avoid funky-ness in the forced collision
            aDice[i].GetComponent<Collider>().enabled = false;
            dDice[i].GetComponent<Collider>().enabled = false;
        }
    }

    // Called every frame until the smashing is "done"
    // All it does it move the dice to the centre based on where they start 
    // Once they reach the centre they either Explode or do nothing
    public void DoTheSmashing()
    {
        bool allDone = true;
        for (int i = 0; i<winners.Count; i++)
        {
            if (aDice[i].transform.position.x < -2f)
            {
                aDice[i].transform.localPosition += Vector3.right * smashSpeed * Time.deltaTime;
                dDice[i].transform.localPosition += Vector3.left * smashSpeed * Time.deltaTime;
                allDone = false;
            }
            else
            {
                if (winners[i] == 0) // Attack Dice Won
                {
                    if (aDice[i].transform.localPosition.x < 0f)
                    {
                        aDice[i].transform.localPosition += Vector3.right * smashSpeed * Time.deltaTime;
                        if (!dDice[i].GetComponent<Explode>().isExploding)
                        {
                            dDice[i].GetComponent<Explode>().ExplodeCube();
                        }
                        allDone = false;
                    }
                    else
                    {
                        aDice[i].transform.localPosition = new Vector3(0, aDice[i].transform.localPosition.y, aDice[i].transform.localPosition.z);
                    }
                }
                else // Defend Dice Won
                {
                    if (dDice[i].transform.localPosition.x > 0f)
                    {
                        dDice[i].transform.localPosition += Vector3.left * smashSpeed * Time.deltaTime;
                        if (!aDice[i].GetComponent<Explode>().isExploding)
                        {
                            aDice[i].GetComponent<Explode>().ExplodeCube();
                        }
                        allDone = false;
                    }
                    else
                    {
                        dDice[i].transform.localPosition = new Vector3(0, dDice[i].transform.localPosition.y, dDice[i].transform.localPosition.z);
                    }
                }
            }
        }
        if (allDone)
        {
            smashing = false;
            // Delayed so the user can acknoledge which dice won 
            Invoke("Finish", postSmashDelay);
        }
    }


    // This method runs once and cleans up all the mess
    // Resets the exploding dice and hides all the dice
    // Resets the lists for the dice - we dont delete the dice as this can cause errors and it is less CPU intensive to disable rather than destroy
    // We finally call the action to signify to everyone else that the dice roll is over - we pass the winners array 
    public void Finish()
    {
        foreach(GameObject dice in dice)
        {
            dice.SetActive(false);
            if (dice.GetComponent<Explode>().isExploding)
            {
                dice.GetComponent<Explode>().DespawnCubes();
            }
        }
        dice = new List<GameObject>();
        aDice = new List<GameObject>();
        dDice = new List<GameObject>();
        if (ActionAfterDiceRoll != null) ActionAfterDiceRoll(winners.ToArray());
    }

    public bool skipAnim = false;
    // The inital method to this madness
    // Dice numbers arent handled here as in the rules you cant have more defender dice than attacker dice (room for error)
    // Dice numbers are handled in the attack handler
    // The method creates all the dice gameobjects and sets the up and then finally gives a "push" (force) to roll them
    public void ThrowDice(int attackDie, int defendDie)
    {
        if (skipAnim)
        {
            List<int> x = new List<int>();
            for (int i = 0; i<defendDie; i++)
            {
                int a = UnityEngine.Random.Range(1, 7);
                int d = UnityEngine.Random.Range(1, 7);
                if (a > d) x.Add(0);
                else x.Add(1);
            }
            if (ActionAfterDiceRoll != null) ActionAfterDiceRoll(x.ToArray());
            return;
        }
        dice = new List<GameObject>();
        aDiceNum = attackDie;
        for (int i = 0; i < attackDie; i++)
        {
            dice.Add(Instantiate(attackerDie, transform));
        }
        for (int i = 0; i < defendDie; i++)
        {
            dice.Add(Instantiate(defenderDie, transform));
        }
        foreach (GameObject obj in dice)
        {
            Vector3 randDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.1f, 0.3f), UnityEngine.Random.Range(-1f, 1f)) * 100f;
            obj.transform.position = new Vector3(0, 20, 0);
            obj.transform.localEulerAngles = randDir;
            obj.GetComponent<Rigidbody>().isKinematic = false;
            obj.GetComponent<Rigidbody>().velocity = randDir;
        }
        thrown = true;
    }
}
