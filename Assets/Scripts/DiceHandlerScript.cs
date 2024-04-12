using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DiceHandlerScript : MonoBehaviour
{
    public List<GameObject> dice;
    bool thrown = false;
    bool moving = false;
    bool smashing = false;
    public GameObject attackerDie;
    public GameObject defenderDie;

    Vector3[] targetPosys;
    List<Quaternion> targetRotys;
    int[] diceOutcomes;
    int aDiceNum = -1;

    List<int> winners;

    List<GameObject> aDice = new List<GameObject>();
    List<GameObject> dDice = new List<GameObject>();

    public Action<int[]> ActionAfterDiceRoll;

    private void Start()
    {
        //ThrowDice(3,1);
    }

    private void Update()
    {
        if (thrown)
        {
            if (moving)
            {
                DiceMoving();
            }
            else
            {
                bool stoppedRolling = true;
                for (int i = 0; i<dice.Count; i++)
                {
                    if (dice[i].transform.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
                    {
                        stoppedRolling = false;
                    }
                }
                if (stoppedRolling)
                {
                    moving = true;
                    GetDiceRoll();
                }
            }
        }
        else if (smashing)
        {
            DoTheSmashing();
        }
    }

    private void GetDiceRoll()
    {
        targetPosys = new Vector3[dice.Count];
        targetRotys = new List<Quaternion>();
        diceOutcomes = new int[dice.Count];
        for (int i = 0; i < dice.Count; i++)
        {
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
            if (i >= aDiceNum)
            {
                dice[i].name += " blue";
            }
            else
            {
                dice[i].name += " red";
            }
            dice[i].GetComponent<Rigidbody>().isKinematic = true;
        }
        int[] aOut = new int[aDiceNum];
        int[] dOut = new int[dice.Count - aDiceNum];
        int aIdx = 0;
        int dIdx = 0;
        // cheeky little insertion sort
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
        for (int i = 0; i < dice.Count; i++)
        {
            dice[i].transform.transform.position = Vector3.Lerp(dice[i].transform.transform.position, targetPosys[i], 2.5f * Time.deltaTime);
            dice[i].transform.transform.localRotation = Quaternion.Lerp(dice[i].transform.transform.localRotation, targetRotys[i], 2.5f * Time.deltaTime);
            float distance = (targetPosys[i] - dice[i].transform.transform.position).magnitude;
            float rDistance = Quaternion.Dot(dice[i].transform.transform.localRotation, targetRotys[i]);
            if (distance < 0.075f && Mathf.Abs(rDistance) >= 0.99f)
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
            
            Invoke("Smash", 0.5f);
        }
    }

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
                dDice[i].GetComponent<Explode>().SpawnCubes();
            }
            else
            {
                winners.Add(1);
                aDice[i].GetComponent<Explode>().SpawnCubes();
            }
            aDice[i].GetComponent<Collider>().enabled = false;
            dDice[i].GetComponent<Collider>().enabled = false;
        }
    }

    float smashSpeed = 40f;
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
            Invoke("Finish", 3f);
        }
    }

    public void Finish()
    {
        foreach(GameObject dice in dice)
        {
            dice.SetActive(false);
        }
        dice = new List<GameObject>();
        aDice = new List<GameObject>();
        dDice = new List<GameObject>();
        if (ActionAfterDiceRoll != null) ActionAfterDiceRoll(winners.ToArray());
    }

    public void ThrowDice(int attackDie, int defendDie)
    {
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
