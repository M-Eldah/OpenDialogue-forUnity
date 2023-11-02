using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LiveActor : MonoBehaviour
{
    public RectTransform rect;
    public Image img;
    public bool left;
    public Actor actor;
   
    public void Stagesetup(Actor a,bool left,float pos)
    {
        actor= a;
        img.sprite = a.expression[0];
        this.left = left;
        if (left) { rect.localScale = new Vector2(rect.localScale.y*-1, rect.localScale.y); }
        changeposition(pos);
    }
    public void changeposition(float position)
    {
       StartCoroutine(QuiteLerpPosition(position));
    }
    public void Hue(float hue)
    {
        StartCoroutine(QuiteLerpBrightness(hue));
    }
    public IEnumerator QuiteLerpPosition(float x)
    {
        Vector2 currentPos = rect.anchoredPosition;
       
        Vector2 wantedPos = new Vector3(x, currentPos.y);
        for (int i = 0; i <= 30; i++)
        {
           
            float v = currentPos.x+(x - currentPos.x)*i/30;
            rect.anchoredPosition = new Vector2(v, 0);
            yield return new WaitForFixedUpdate();
            
        }
        
    }
    public void Flip()
    {
        StartCoroutine(flip());
    }
    public IEnumerator flip()
    {
        float t = rect.localScale.x*-1;
       
        for (int i = 0; i <= 30; i++)
        {
           
            float v = rect.localScale.x + (t - rect.localScale.x) * i / 30;
            rect.localScale = new Vector2(v, rect.localScale.y);
            yield return new WaitForFixedUpdate();

        }

    }
    public IEnumerator QuiteLerpBrightness(float x)
    {
        float currentBrightness = rect.gameObject.GetComponent<Image>().color.b;
        for (int i = 0; i <= 10; i++)
        {
          
            float v = currentBrightness + (x - currentBrightness) * i / 10;
            rect.gameObject.GetComponent<Image>().color = new Color(v,v,v);
            yield return new WaitForFixedUpdate();

        }

    }
}
