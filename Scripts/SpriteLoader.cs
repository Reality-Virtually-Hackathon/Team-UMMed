using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{

    [SerializeField]
    GameObject unit;
    [SerializeField]
    Material mat;

    private SpriteRenderer[] SRs = new SpriteRenderer[266];

    float minW, maxW, minH, maxH, minD, maxD, threshold;

    // Use this for initialization
    void Start()
    {
        loadSprite();
        minD = minH = minW = 0;
        maxD = maxH = maxW = 1;
        threshold = 0.2f;
        setWidth(0, 1);
        setHeight(0, 1);
        setDepth(0, 132);
        setThreshold(0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            setDepth(0, 131);

    }

    void loadSprite()
    {
        for (int i = 0; i < 266; i += 2)
        {
            Sprite sprite = (Sprite)Resources.Load("Pics1/" + "knee" + i.ToString(), typeof(Sprite));
            GameObject temp = GameObject.Instantiate(unit, transform.position, Quaternion.identity);
            temp.transform.Translate(Vector3.back * i / 100f);
            temp.GetComponent<SpriteRenderer>().sprite = sprite;
            temp.transform.localScale = new Vector3(0.5F, 0.5F, 0.5F);
            temp.transform.parent = this.transform;

            SRs[i / 2] = temp.GetComponent<SpriteRenderer>();
        }
    }

    public void setWidth(float min, float max)
    {
        if (min < 0 || max > 1)
            return;
        minW = min;
        maxW = max;
        mat.SetFloat("_XMin", min);
        mat.SetFloat("_XMax", max);

        //mat.GetFloat("_XMax");
    }

    public float getWidthMin()
    {
        return mat.GetFloat("_XMin");
    }

    public float getWidthMax()
    {
        return mat.GetFloat("_XMax");
    }




    public void setHeight(float min, float max)
    {
        if (min < 0 || max > 1)
            return;
        minH = min;
        maxH = max;
        mat.SetFloat("_YMin", min);
        mat.SetFloat("_YMax", max);
    }

    public float getHeightMin()
    {
        return mat.GetFloat("_YMin");
    }

    public float getHeightMax()
    {
        return mat.GetFloat("_YMax");
    }


    public void setDepth(int min, int max)
    {
        //if (min < 0 || max > 1)
        //    return;
        //minD = min;
        //maxD = max;
        //for(int i = 0; i < 134; ++i)
        //{
        //    if (i < 133 * min || i > 133 * max)
        //        SRs[i].enabled = false;
        //    else
        //        SRs[i].enabled = true;
        //}


        for (int i = 0; i <= 131; ++i)
        {

            if (i >= min && i <= max)
                SRs[i].GetComponent<SpriteRenderer>().enabled = true;
            else
                SRs[i].GetComponent<SpriteRenderer>().enabled = false;
        }


    }

    public int getminDepth()
    {
        int mindepthSlide = 0;

        for (int j = 0; j <= 131; j++)
        {
            if (SRs[j].GetComponent<SpriteRenderer>().enabled == true)
            {
                mindepthSlide = j;
                break;
            }
        }
        return mindepthSlide;
    }

    public int getmaxDepth()
    {
        int maxdepthSlide = 131;
        for (int k = 131; k >= 0; k--)
        {

            if (SRs[k].GetComponent<SpriteRenderer>().enabled == true)
            {
                maxdepthSlide = k;
                break;
            }
        }
        return maxdepthSlide;
    }




    public void setThreshold(float input)
    {
        if (input > 1 || input < 0)
            return;
        threshold = input;
        mat.SetFloat("_offsetValue", input);
    }
}
