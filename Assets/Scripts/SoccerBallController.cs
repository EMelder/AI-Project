using UnityEngine;



public class SoccerBallController : MonoBehaviour
{
    public GameObject area;
    [HideInInspector]
    public SoccerEnvController envController;
    public string purpleGoalTag; //will be used to check if collided with purple goal
    public string blueGoalTag; //will be used to check if collided with blue goal
    public string purpleAgentTag;
    public string blueAgentTag;
    [HideInInspector]
    public GameObject LastKick;

    void Start()
    {
        envController = area.GetComponent<SoccerEnvController>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(purpleGoalTag)) //ball touched purple goal
        {
            envController.GoalTouched(Team.Blue);
        }
        else if (col.gameObject.CompareTag(blueGoalTag)) //ball touched blue goal
        {
            envController.GoalTouched(Team.Purple);
        }
        else if (col.gameObject.CompareTag(purpleAgentTag))
        {
            if (LastKick != null && LastKick.CompareTag(col.gameObject.tag) && LastKick.name != col.gameObject.name)
            {
                Debug.Log("There was a pass between Purple\n");
                col.gameObject.GetComponent<AgentSoccer>().AddReward(0.2f);
                LastKick.GetComponent<AgentSoccer>().AddReward(0.2f);
            }
            LastKick = col.gameObject.GetComponent<GameObject>();
        }
        else if (col.gameObject.CompareTag(blueAgentTag))
        {
            if (LastKick != null && LastKick.CompareTag(col.gameObject.tag) && LastKick.name != col.gameObject.name)
            {
                Debug.Log("There was a pass between Blue\n");
                col.gameObject.GetComponent<AgentSoccer>().AddReward(0.2f);
                LastKick.GetComponent<AgentSoccer>().AddReward(0.2f);
            }
            LastKick = col.gameObject.GetComponent<GameObject>();
        }
    }
}
