using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIScore : SceneMono<UIScore>
{
    // Start is called before the first frame update
    public int score = 0;

    [SerializeField]
    GameObject[] tenDigit, oneDigit, slots;

    public int carry = 5;

    public static int[][] digital_map = {
        new int[]{ 1,1,1,0,1,1,1 }, //0
        new int[]{ 0,0,1,0,0,1,0 },
        new int[]{ 1,0,1,1,1,0,1 },
        new int[]{ 1,0,1,1,0,1,1 }, // 3
        new int[]{ 0,1,1,1,0,1,0 },
        new int[]{ 1,1,0,1,0,1,1 },
        new int[]{ 1,1,0,1,1,1,1 }, // 6
        new int[]{ 1,0,1,0,0,1,0 },
        new int[]{ 1,1,1,1,1,1,1 },
        new int[]{ 1,1,1,1,0,1,1 },
        new int[]{ 1,1,1,1,1,1,0 }, // A
        new int[]{ 0,1,0,1,1,1,0 }, // B
        new int[]{ 1,1,0,0,1,0,1 }, // C
        new int[]{ 0,0,1,1,1,1,1 }, // D
        new int[]{ 1,1,0,1,1,0,1 }, // E
        new int[]{ 1,1,0,1,1,0,1 }, // F
    };

    new void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }
    public void UpdateScore(int score)
    {
        this.score = score;

        // Plan B
        int p3 = score / (carry * carry);
        int itr = 0;
        var rb = ResBank.FindMe();
        while (p3-- > 0)
        {
            var sl = slots[itr++];
            sl.SetActive(true);
            var sr = sl.GetComponent<SpriteRenderer>();
            sr.sprite = rb.SP_carry[2];
            sr.color = Color.green;
            sr.DOColor(Color.white, 0.3f);
        }
        int p2 = score % (carry * carry) / carry;
        while (p2-- > 0)
        {
            var sl = slots[itr++];
            sl.SetActive(true);
            var sr = sl.GetComponent<SpriteRenderer>();
            sr.sprite = rb.SP_carry[1];
            sr.color = Color.green;
            sr.DOColor(Color.white, 0.3f);
        }
        int p1 = score % carry;
        while (p1-- > 0)
        {
            var sl = slots[itr++];
            sl.SetActive(true);
            var sr = sl.GetComponent<SpriteRenderer>();
            sr.sprite = rb.SP_carry[0];
            sr.color = Color.green;
            sr.DOColor(Color.white, 0.3f);
        }
        while (itr < slots.Length)
        {
            slots[itr++].SetActive(false);
        }

        // int ten = score / (score > 100 ? 16 : 10);
        // if (ten <= 0)
        //     foreach (var x in tenDigit) x.SetActive(false);
        // else for (int i = 0; i < digital_map[ten].Length; ++i)
        //     {
        //         if (digital_map[ten][i] > 0)
        //         {
        //             var go = tenDigit[i];
        //             go.SetActive(true);
        //             var gosr = go.GetComponent<SpriteRenderer>();
        //             gosr.color = Color.green;
        //             gosr.DOColor(Color.white, 0.3f);
        //         }
        //         else tenDigit[i].SetActive(false);
        //     }
        // int one = score % (score > 100 ? 16 : 10);
        // for (int i = 0; i < digital_map[one].Length; ++i)
        // {
        //     if (digital_map[one][i] > 0)
        //     {
        //         var go = oneDigit[i];
        //         go.SetActive(true);
        //         var gosr = go.GetComponent<SpriteRenderer>();
        //         gosr.color = Color.green;
        //         gosr.DOColor(Color.white, 0.3f);

        //     }
        //     else oneDigit[i].SetActive(false);
        // }
    }
    public void DeltaScore(int delta)
    {
        score += delta;
        UpdateScore(score);
    }
}
