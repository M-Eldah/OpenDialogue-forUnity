using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class stageDirection : MonoBehaviour
{
    public static stageDirection instance;
    public int cid;
    public bool left, mmlast;
    public Actor[] actors;
    public List<LiveActor> Live;
    public GameObject actorPrefab;
    public bool Spawn;
    public bool flip;
    public bool active;
    // Start is called before the first frame update
    public void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Spawn)
        {
            //trial spawn orders
            spawnActors(cid, left, mmlast);
        }
        if (flip)
        {
            Live[cid].Flip();
            flip = false;
        }
        if (active)
        {
            active = false;
            setActive(cid);
        }
    }
    public string actorName(int n )
    {
        return actors[n].name;
    }
    public void setActive(int n)
    {
        Live[Stageid(n)].gameObject.transform.SetSiblingIndex(Live[Stageid(n)].gameObject.transform.parent.childCount - 1);
        Debug.Log("Setting who is active");
        for (int i = 0; i < Live.Count; i++)
        {
            float brightness = i == Stageid(n) ? 1 : 0.5f;
            Debug.Log("Brightness value: "+brightness);
            Live[i].Hue(brightness);
           
        }
    }

    /// <summary>
    /// The Method for spawning new actors on the stage and diffrent interactions Between diffrent ones
    /// </summary>
    /// <param name="a"></param>
    /// <param name="side">, effect after the first actor Right side by default</param>
    /// <param name="mmLast"> should the last actor which entered before this one switch sodes</param>
    public void spawnActors(int n, bool left = false, bool mmLast = true)
    {
        float p = left ? -702 : 702;
        if (Live.Count == 0)
        {
            addActor(actors[n], left, p);
            Spawn = false;
        }
        else
        {
            if (Live.Where(x => x.left == left).Count() == 0)
            {
                addActor(actors[n], left, p);
                Spawn = false;
            }
            else
            {
                if (Live.Where(x => x.left == left).Count() > 1)
                {
                    for (int i = 0; i < Live.Count; i++)
                    {
                        if (Live[i].left == left)
                        {
                            if (mmLast)
                            {
                                FlipSideint(i);
                            }
                            else
                            {
                                MakeRoomint(i);
                            }
                        }
                    }
                    addActor(actors[n], left, p);
                    Spawn = false;
                }
                else
                {
                    if (mmLast)
                    {
                        int ind = Live.IndexOf(Live.Where(x => x.left == left).FirstOrDefault());
                        FlipSideint(ind);
                        addActor(actors[n], left, p);
                        Spawn = false;
                    }
                    else
                    {
                        if (Live.Where(x => x.left == left).Count() != 0)
                        {
                            int ind = Live.IndexOf(Live.Where(x => x.left == left).FirstOrDefault());
                            MakeRoomint(ind);
                        }
                        addActor(actors[n], left, p);
                        Spawn = false;
                    }
                }
            }
        }
    }
    public int Stageid(int n)
    {
      
        return Live.IndexOf(Live.Where(x => x.actor == actors[n]).FirstOrDefault());
    }
    #region internal access where they enter the live list directly
    private void MakeRoomint(int n)
    {
        float pos = Mathf.Abs(Live[n].rect.anchoredPosition.x) - 252;
        pos = Live[n].left ? pos * -1 : pos;
        Live[n].changeposition(pos);
    }
    private void FlipSideint(int n)
    {
        float c = Live.Where(actor => actor.left != Live[n].left).Count();
        float pos = 702 - c * 252;
        pos = Live[(n)].left ? pos : pos * -1;
        FlipActorint((n));
        MoveActorint((n), pos);
        Live[(n)].left = !Live[(n)].left;
    }
    private void MoveActorint(int n, float p)
    {
        Live[(n)].changeposition(p);
    }

    private void FlipActorint(int n)
    {
        Live[(n)].Flip();
    }
    #endregion
    #region external access by contverting the actor id to postion on the live list
    public void MakeRoom(int n)
    {
        float pos = Mathf.Abs(Live[Stageid(n)].rect.anchoredPosition.x) - 252;
        pos = Live[Stageid(n)].left ? pos * -1 : pos;
        Live[Stageid(n)].changeposition(pos);
    }
   
    public void FlipSide(int n)
    {
        float c = Live.Where(actor => actor.left != Live[n].left).Count();
        float pos = 702 - c * 252;
        pos = Live[Stageid(n)].left ? pos : pos * -1;
        FlipActor(Stageid(n));
        MoveActor(Stageid(n), pos);
        Live[Stageid(n)].left = !Live[Stageid(n)].left;
    }
    
    public void SetLvl(int n,int i)
    {
        Live[Stageid(n)].gameObject.transform.SetSiblingIndex(i);
    }
    
    public void MoveActor(int n, float p)
    {
        Live[Stageid(n)].changeposition(p);
    }

    public void FlipActor(int n)
    {
        Live[Stageid(n)].Flip();
    }
    //Clear actors at dialogue end
    public void Clearactors()
    {
        foreach (var act in Live)
        {
            act.transform.SetParent(null);
            Destroy(act.transform.gameObject);
        }
        Live.Clear();

    }
    #endregion
    private void addActor(Actor a, bool left, float pos)
    {
        GameObject actor = Instantiate(actorPrefab, transform);
        actor.transform.name = a.name;

        int s = pos > 0 ? 1400 : -1400;
        actor.GetComponent<RectTransform>().anchoredPosition = new Vector2(s, 0);
        actor.GetComponent<LiveActor>().Stagesetup(a, left, pos);
        Live.Add(actor.GetComponent<LiveActor>());
    }
}