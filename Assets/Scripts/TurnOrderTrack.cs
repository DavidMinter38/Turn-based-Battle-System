using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderTrack : MonoBehaviour
{
    [SerializeField]
    Image[] trackImages;

    [SerializeField]
    Sprite emptySprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTrack(ArrayList sprites)
    {
        int trackCounter = 0;
        foreach (Sprite sprite in sprites)
        {
            trackImages[trackCounter].sprite = sprite;
            trackCounter++;
        }
        for(int i = trackCounter; i<trackImages.Length; i++)
        {
            trackImages[i].sprite = emptySprite;
        }
    }
}
