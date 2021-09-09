using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Villager : MonoBehaviour
{
    bool initialised;
    
    [SerializeField]
    int age;
    bool gender;
    [SerializeField]
    Home home;
    bool nightTime;
    string named;

    float maxHealth;
    float health;
    float hunger = 0;
    bool underAttack;

    JobList jobs;
    [SerializeField]
    int jobNum = -1;
    [SerializeField]
    Transform hand;

    float carryCap;
    [SerializeField]
    Transform shoulder;

    Gore gore;

    AudioSource aud;
    AudioClip toolSound;
    Animator anim;
    NavMeshAgent agent;
    Collider col;
    [SerializeField]
    Transform target;
    public string currentTask = "Idling";
    public float idleTimer;

    private void Start()
    {
        jobs = FindObjectOfType<JobList>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        gore = GetComponent<Gore>();
        if (!initialised)
        {
            Initilise();
            SetJob(jobNum);
        }
    }

    private void Update()
    {

        //Decide task
        if (underAttack){
            currentTask = "Fleeing";
        } else if (hunger < 0 && currentTask == "Idling")
        {
            if (shoulder.GetChild(0).tag == "Grain")
            {
                currentTask = "Eating";
            } else
            {
                currentTask = "Getting Food";
            }
        } else if (jobNum > -1 && !nightTime)
        {
            if (!col.enabled)
            {
                Sleep(false);
            }
            currentTask = "Working"; // need to add enumerator for idle walking between jobs
        }
        else
        {
            if (target == null || target.tag != "Barn")
            {
                currentTask = "Going to Bed";
            } else
            {
                currentTask = "Working";
            }
        }


        //Do task
        switch (currentTask)
        {
            case "Fleeing":
                {
                    break;
                }
            case "Fighting":
                {
                    break;
                }
            case "Going to Bed":
                {
                    //Go to bed unless carrying raw resource
                    if (target == null || target.tag == jobs.ReturnStack(jobNum) || target.tag == jobs.ReturnResource(jobNum))
                    {
                        if (target != null)
                        {
                            DepositResource();
                        }
                        target = home.transform;
                    }

                    //when done tools away, head home
                    ToolVisible(false);

                    if (InRange(1.5f, home.transform))
                    {
                        home.EnteredHome(this, true);
                        Sleep(true);
                    }
                    break;
                }
            case "Idling":
                {
                    break;
                }
            case "Eating":
                {
                    break;
                }
            case "Getting Food":
                {
                    break;
                }
            case "Working":
                {
                    //Make Tool visible
                    hand.GetChild(0).gameObject.SetActive(true);

                    //Locate work tool
                    if (target == null || target == home.transform)
                    {
                        target = Locate.NearestWithTag(transform.position, jobs.ReturnStack(jobNum));
                    }
                    if (target == null || target == home.transform)
                    {
                        target = Locate.NearestWithTag(transform.position, jobs.ReturnResource(jobNum));
                    }

                    //Complete actions
                    if (target != null && target != home.transform)
                    {
                        if (target.tag == "Barn")
                        {
                            ToolVisible(false);
                            anim.SetBool("Carrying", true);
                            anim.SetBool("Resource", false);
                            agent.speed = 0.75f;
                        } else
                        {
                            ToolVisible(true);
                            anim.SetBool("Carrying", false);
                            agent.speed = 1.5f;
                        }

                        if (InRange(1.5f, target))
                        {
                            if (target.tag == jobs.ReturnResource(jobNum)) //If destroyable resource, damage resource
                            {
                                anim.SetBool("Resource", true);
                            }
                            else
                            {
                                if (target.tag == jobs.ReturnStack(jobNum)) // If harvested resource, put the object on your shoulder and take it to the barn
                                {
                                    target.parent = shoulder;
                                    target.localPosition = Vector3.zero;
                                    target.localRotation = Quaternion.Euler(Vector3.zero);
                                    Destroy(target.GetComponent<Rigidbody>());
                                    target = Locate.NearestWithTag(transform.position, "Barn");
                                }
                                else if (target.tag == "Barn") // If at the barn, drop off resource and remove target lock
                                {
                                    DepositResource();
                                }
                            }
                        }
                    }
                    break;
                }
        }
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up, named + "\n" + "Currently: " + currentTask);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Rock")
        {
            Rigidbody rockRB = collision.transform.GetComponent<Rigidbody>();
            if(rockRB.velocity.magnitude > 1f)
            {
                gore.ActivateRagdoll(true);
            }
        }
    }

    //mine resource
    private void ReduceResource()
    {
        target.GetComponent<Resource>().gathered -= 25;
        aud.PlayOneShot(toolSound);
    }

    private void ToolNoise()
    {
        aud.PlayOneShot(toolSound);
    }

    private void DepositResource()
    {
        if (shoulder.childCount > 0)
        {
            if(target.tag == "Barn")
            {
                shoulder.GetChild(0).GetComponent<Stockpile>().TransferToBarn();
            }
            shoulder.GetChild(0).GetComponent<BeingUsed>().StopUsing();
            shoulder.GetChild(0).gameObject.AddComponent<Rigidbody>();
            shoulder.GetChild(0).transform.parent = null;
        }
        anim.SetBool("Resource", false);
        anim.SetBool("Carrying", false);
        target.GetComponent<BeingUsed>().StopUsing();
        target = null;
    }

    public void Sleep(bool visible)
    {
        transform.GetChild(1).gameObject.SetActive(!visible);
        col.enabled = !visible;
    }

    private void ToolVisible(bool active)
    {
        hand.GetChild(0).gameObject.SetActive(active);
    }

    public bool InRange(float dist, Transform dest)
    {
        if (Vector3.Distance(transform.position, dest.position) > dist) //If not close enough to target
        {
            agent.SetDestination(target.position);
            return false;
        } else
        {
            return true;
        }
    }

    public void GetFromBarn(string item, float amount)
    {
        Transform barn = Locate.NearestWithTag(transform.position, "Barn");
        GameObject grabbed = Instantiate(barn.parent.GetComponent<Barn>().GetPrefab(item), shoulder.position, shoulder.rotation);
        grabbed.transform.parent = shoulder;
        grabbed.GetComponent<Stockpile>().amount = barn.parent.GetComponent<Barn>().PickUp(amount, item);
    }

    public void TimeofDay(bool night)
    {
        nightTime = night;
    }

    public void Initilise()
    {
        if (Random.Range(1, 10) > 6)
        {
            gender = true;
        } else
        {
            gender = false;
        }
        named = NameGenerator.GetName(gender);
        gameObject.name = named;

        Home[] homes = FindObjectsOfType<Home>();
        
        foreach (Home house in homes)
        {
            if (!house.InUse() && home == null)
            {
                house.EnteredHome(this, true);
                house.StartUsing();
                home = house;
            }
        }

        if (age < 14f)
        {
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            maxHealth = 5f;
            health = 5f;
            carryCap = 10f;
            //Debug.Log("A " + (gender ? "boy" : "girl") + " named " + named + " was born!");
        } else
        {
            maxHealth = 20f;
            health = 20f;
            carryCap = 50f;
            //Debug.Log("A " + (gender ? "man" : "woman") + " named " + named + " joins the tribe!");
        }
    }

    public void SetJob(int jobNo)
    {
        if (jobNo > -1)
        {
            GameObject tool = Instantiate(jobs.ReturnTool(jobNo), hand, true);
            tool.transform.localPosition = Vector3.zero;
            tool.transform.localRotation = Quaternion.Euler(0, 0, 0);
            toolSound = jobs.ReturnSound(jobNo);
        } else
        {
            if (jobNum != -1)
            {
                Destroy(hand.GetChild(0));
            }
        }
        jobNum = jobNo;
    }
}
