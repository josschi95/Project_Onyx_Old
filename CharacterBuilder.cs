using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBuilder : MonoBehaviour
{
    public List<GameObject> activeObjects = new List<GameObject>();
    


    [HideInInspector] public bool isMale = true;
    [HideInInspector] public bool isHuman = true;

    public GameObject[] defaultMale;
    public GameObject[] defaultFemale;

    [Header("Facial Features")]
    public GameObject[] maleHeads;
    public GameObject[] femaleHeads;
    public GameObject[] elfEars;
    public GameObject[] facialHair;
    public GameObject[] maleEyebrows;
    public GameObject[] femaleEyebrows;
    public GameObject[] hairStyles;

    [Header("Sliders")]
    public Slider headSlider;
    public Slider elfEarSlider;
    public Slider facialHairSlider;
    public Slider eyebrowSlider;
    public Slider hairStyleSlider;
    public Slider skinSlider;
    public Slider hairColorSlider;
    public Slider bodyArtSlider;

    #region - Colors -
    [Header("Material")]
    public Material mat;

    [Header("Skin Colors")]
    public Color paleSkin = new Color(0.9607844f, 0.7843138f, 0.7294118f);
    public Color whiteSkin = new Color(1f, 0.8000001f, 0.682353f);
    public Color brownSkin = new Color(0.8196079f, 0.6352941f, 0.4588236f);
    public Color blackSkin = new Color(0.5647059f, 0.4078432f, 0.3137255f);

    [Header("Scar Colors")]
    public Color paleScar = new Color(0.8745099f, 0.6588235f, 0.6313726f);
    public Color whiteScar = new Color(0.9294118f, 0.6862745f, 0.5921569f);
    public Color brownScar = new Color(0.6980392f, 0.5450981f, 0.4f);
    public Color blackScar = new Color(0.4235294f, 0.3176471f, 0.282353f);

    [Header("Stubble Colors")]
    public Color paleStubble = new Color(0.8627452f, 0.7294118f, 0.6862745f);
    public Color whiteStubble = new Color(0.8039216f, 0.7019608f, 0.6313726f);
    public Color brownStubble = new Color(0.6588235f, 0.572549f, 0.4627451f);
    public Color blackStubble = new Color(0.3882353f, 0.2901961f, 0.2470588f);

    
    [Header("Hair Colors")]
    public Color hair1 = new Color(0.2196079f, 0.2196079f, 0.2196079f); //black, if not close to it
    public Color hair2 = new Color(0.1764706f, 0.1686275f, 0.1686275f); //dark brown
    public Color hair3 = new Color(0.3098039f, 0.254902f, 0.1764706f); //brown
    public Color hair4 = new Color(0.3843138f, 0.2352941f, 0.0509804f); // half orange/half blonde? light brown
    public Color hair5 = new Color(0.6862745f, 0.4f, 0.2352941f); //red/orange
    public Color hair6 = new Color(0.5450981f, 0.427451f, 0.2156863f); //kinda dark blonde
    public Color hair7 = new Color(0.8470589f, 0.4666667f, 0.2470588f); //bright orange?
    public Color hair8 = new Color(0.8313726f, 0.6235294f, 0.3607843f); //yellow blonde
    public Color hair9 = new Color(0.8980393f, 0.7764707f, 0.6196079f); //pale blonde
    public Color hair10 = new Color(0.6196079f, 0.6196079f, 0.6196079f); // more gray 
    public Color hair11 = new Color(0.8000001f, 0.8196079f, 0.8078432f); // gray?
    public Color hair12 = new Color(0.9764706f, 0.9686275f, 0.9568628f); //white
    
    [Header("Body Art Colors")]
    public Color bodyArt0 = new Color(0.0509804f, 0.6745098f, 0.9843138f);
    public Color bodyArt1 = new Color(0.7215686f, 0.2666667f, 0.2666667f);
    public Color bodyArt2 = new Color(0.4483752f, 0.03537735f, 0.5f);
    public Color bodyArt3 = new Color(0.1348047f, 0.4433962f, 0.119215f);
    public Color bodyArt4 = new Color(0.6981132f, 0.4815834f, 0.05598078f);
    public Color bodyArt5 = new Color(1f, 1f, 1f);
    public Color bodyArt6 = new Color(0f, 0f, 0f);
    #endregion

    //public Color //lots of these

    private void Start()
    {
        SetMale();

        elfEarSlider.enabled = false;

        headSlider.onValueChanged.AddListener(delegate { ChangeHead((int)headSlider.value); });
        elfEarSlider.onValueChanged.AddListener(delegate { ChangeEars((int)elfEarSlider.value); });
        facialHairSlider.onValueChanged.AddListener(delegate { ChangeFacialHair((int)facialHairSlider.value); });
        eyebrowSlider.onValueChanged.AddListener(delegate { ChangeEyebrows((int)eyebrowSlider.value); });
        hairStyleSlider.onValueChanged.AddListener(delegate { ChangeHairStyle((int)hairStyleSlider.value); });
        skinSlider.onValueChanged.AddListener(delegate { ChangeSkinColor((int)skinSlider.value); });
        hairColorSlider.onValueChanged.AddListener(delegate { ChangeHairColor((int)hairColorSlider.value); });
        bodyArtSlider.onValueChanged.AddListener(delegate { ChangeBodyArtColor((int)bodyArtSlider.value); });

    }

    public void ChangeGender()
    {
        isMale = !isMale;

        if (isMale)
        {
            SetMale();
        }
        else
        {
            SetFemale();
        }
    }

    public void ChangeRace()
    {
        isHuman = !isHuman;

        if (isHuman)
        {
            elfEarSlider.enabled = false;

            foreach (GameObject gameObject in elfEars)
            {
                RemoveItem(gameObject);
            }
        }
        else
        {
            elfEarSlider.enabled = true;
            ChangeEars(0);
        }
    }

    void ChangeHead(int headNumber)
    {
        if (isMale)
        {
            foreach (GameObject go in maleHeads)
                if (headNumber == System.Array.IndexOf(maleHeads, go))
                {
                    AddItem(go);
                }
                else
                {
                    RemoveItem(go);
                }
        }
        else
        {
            foreach (GameObject go in femaleHeads)
                if (headNumber == System.Array.IndexOf(femaleHeads, go))
                {
                    AddItem(go);
                }
                else
                {
                    RemoveItem(go);
                }
        }
    }

    void ChangeEars(int earNumber)
    {
        foreach (GameObject go in elfEars)
            if (earNumber == System.Array.IndexOf(elfEars, go))
            {
                AddItem(go);
            }
            else
            {
                RemoveItem(go);
            }
    }

    void ChangeFacialHair(int beardNumber)
    {
        foreach (GameObject go in facialHair)
            if (beardNumber == System.Array.IndexOf(facialHair, go))
            {
                AddItem(go);
            }
            else
            {
                RemoveItem(go);
            }
    }

    void ChangeEyebrows(int browNumber)
    {
        if (isMale)
        {
            foreach (GameObject go in maleEyebrows)
                if (browNumber == System.Array.IndexOf(maleEyebrows, go))
                {
                    AddItem(go);
                }
                else
                {
                    RemoveItem(go);
                }
        }
        else
        {
            foreach (GameObject go in femaleEyebrows)
                if (browNumber == System.Array.IndexOf(femaleEyebrows, go))
                {
                    AddItem(go);
                }
                else
                {
                    RemoveItem(go);
                }
        }
    }

    void ChangeHairStyle(int hairNumber)
    {
        foreach (GameObject go in hairStyles)
            if (hairNumber == System.Array.IndexOf(hairStyles, go))
            {
                AddItem(go);
            }
            else
            {
                RemoveItem(go);
            }
    }

    void ChangeSkinColor(int colorNumber)
    {
        Color skinColor = Color.black;
        Color scarColor = Color.black;
        Color stubbleColor = Color.black;

        switch (colorNumber)
        {
            case 0:
                skinColor = paleSkin;
                scarColor = paleScar;
                stubbleColor = paleStubble;
                break;
            case 1:
                skinColor = whiteSkin;
                scarColor = whiteScar;
                stubbleColor = whiteStubble;
                break;
            case 2:
                skinColor = brownSkin;
                scarColor = brownScar;
                stubbleColor = brownStubble;
                break;
            case 3:
                skinColor = blackSkin;
                scarColor = blackScar;
                stubbleColor = blackStubble;
                break;

        }
        //set the skin color
        mat.SetColor("_Color_Skin", skinColor);

        // set stubble color
        mat.SetColor("_Color_Stubble", stubbleColor);

        // set scar color
        mat.SetColor("_Color_Scar", scarColor);
    }
    
    void ChangeHairColor(int colorNumber)
    {
        Color hairColor = Color.black;

        switch (colorNumber)
        {
            case 1:
                {
                    hairColor = hair1;
                    break;
                }
            case 2:
                {
                    hairColor = hair2;
                    break;
                }
            case 3:
                {
                    hairColor = hair3;
                    break;
                }
            case 4:
                {
                    hairColor = hair4;
                    break;
                }
            case 5:
                {
                    hairColor = hair5;
                    break;
                }
            case 6:
                {
                    hairColor = hair6;
                    break;
                }
            case 7:
                {
                    hairColor = hair7;
                    break;
                }
            case 8:
                {
                    hairColor = hair8;
                    break;
                }
            case 9:
                {
                    hairColor = hair9;
                    break;
                }
            case 10:
                {
                    hairColor = hair10;
                    break;
                }
            case 11:
                {
                    hairColor = hair11;
                    break;
                }
            case 12:
                {
                    hairColor = hair12;
                    break;
                }
        }

        mat.SetColor("_Color_Hair", hairColor);
    }
    
    void ChangeBodyArtColor(int colorNumber)
    {
        Color artColor = Color.black;

        switch (colorNumber)
        {
            case 0:
                {
                    artColor = bodyArt0;
                    break;
                }
            case 1:
                {
                    artColor = bodyArt1;
                    break;
                }
            case 2:
                {
                    artColor = bodyArt2;
                    break;
                }
            case 3:
                {
                    artColor = bodyArt3;
                    break;
                }
            case 4:
                {
                    artColor = bodyArt4;
                    break;
                }
            case 5:
                {
                    artColor = bodyArt5;
                    break;
                }
            case 6:
                {
                    artColor = bodyArt6;
                    break;
                }
        }

        mat.SetColor("_Color_BodyArt", artColor);

    }


    void AddItem(GameObject gameObject)
    {
        gameObject.SetActive(true);
        activeObjects.Add(gameObject);
    }

    void RemoveItem(GameObject gameObject)
    {
        gameObject.SetActive(false);
        activeObjects.Remove(gameObject);
    }


    void SetMale()
    {
        foreach (GameObject gameObject in activeObjects)
        {
            gameObject.SetActive(false);
        }

        activeObjects.Clear();

        
        foreach (GameObject gameObject in defaultMale)
        {
            AddItem(gameObject);
        }
        
        ChangeHead((int)headSlider.value);

        if (!isHuman)
        {
            ChangeEars((int)elfEarSlider.value);
        }

        facialHairSlider.enabled = true;

        eyebrowSlider.maxValue = 9;
        ChangeEyebrows((int)eyebrowSlider.value);

        ChangeHairStyle((int)hairStyleSlider.value);

        ChangeSkinColor((int)skinSlider.value);

        ChangeHairColor((int)hairColorSlider.value);

        ChangeBodyArtColor((int)bodyArtSlider.value);
    }

    void SetFemale()
    {
        foreach (GameObject gameObject in activeObjects)
        {
            gameObject.SetActive(false);
        }

        activeObjects.Clear();

        foreach (GameObject gameObject in defaultFemale)
        {
            AddItem(gameObject);
        }

        ChangeHead((int)headSlider.value);

        if (!isHuman)
        {
            ChangeEars((int)elfEarSlider.value);
        }

        facialHairSlider.enabled = false;

        ChangeFacialHair(18);

        eyebrowSlider.maxValue = 6;
        if (eyebrowSlider.value > 6)
        {
            eyebrowSlider.value = 6;
        }
        ChangeEyebrows((int)eyebrowSlider.value);

        ChangeHairStyle((int)hairStyleSlider.value);

        ChangeSkinColor((int)skinSlider.value);

        ChangeHairColor((int)hairColorSlider.value);

        ChangeBodyArtColor((int)bodyArtSlider.value);
    }


}