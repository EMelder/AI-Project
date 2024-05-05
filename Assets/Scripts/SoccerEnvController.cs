using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SoccerEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentSoccer Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }


    public enum Paradime
    {
        Marginal,
        Bulk,
        Hybrid
    }



    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    /// <summary>
    /// The area bounds.
    /// </summary>

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>

    public Paradime paradime;

    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 m_BallStartingPos;

    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SoccerSettings m_SoccerSettings;


    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    private int m_ResetTimer;
    public float KickReward = 0.1f;
    public int BlueScore;
    public int PurpleScore;

    void Start()
    {

        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_PurpleAgentGroup.RegisterAgent(item.Agent);
            }
        }
        //ResetScene();
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            if(BlueScore-PurpleScore > 0)
            {
                Debug.Log("Blue wins with score of " + BlueScore.ToString() + " vs. " + PurpleScore.ToString() + "\n");
            }
            else if(BlueScore-PurpleScore< 0)
            {
                Debug.Log("Purple wins with score of "+ BlueScore.ToString() + " vs. " + PurpleScore.ToString() + "\n");
            }
            if (paradime == Paradime.Bulk)
            {
                m_BlueAgentGroup.AddGroupReward(BlueScore - PurpleScore);
                m_PurpleAgentGroup.AddGroupReward(-(BlueScore - PurpleScore));
            }
            else if (paradime == Paradime.Hybrid)
            {
                m_BlueAgentGroup.AddGroupReward(0.5f * (BlueScore - PurpleScore));
                m_PurpleAgentGroup.AddGroupReward(-0.5f * (BlueScore - PurpleScore));
            }
            else if (paradime == Paradime.Marginal)
            {
                m_BlueAgentGroup.AddGroupReward(0.1f * (BlueScore - PurpleScore));
                m_PurpleAgentGroup.AddGroupReward(-0.1f * (BlueScore - PurpleScore));
            }

            m_BlueAgentGroup.GroupEpisodeInterrupted();
            m_PurpleAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }


    public void ResetBall()
    {

        var randomPosX = Random.Range(-2.5f, 2.5f);
        var randomPosZ = Random.Range(-2.5f, 2.5f);

        ball.transform.position = m_BallStartingPos + new Vector3(randomPosX, 0f, randomPosZ);
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

    }

    public void GoalTouched(Team scoredTeam)
    {
        if (scoredTeam == Team.Blue)
        {
            Debug.Log("Blue Team scored!\n");
            BlueScore++;
            Debug.Log("Score: (" + BlueScore.ToString() + ")B v. (" + PurpleScore.ToString() + ")P\n");
            m_BlueAgentGroup.AddGroupReward(1);
            m_PurpleAgentGroup.AddGroupReward(-1);
        }
        else
        {

            Debug.Log("Purple Team scored!\n");
            PurpleScore++;
            Debug.Log("Score: (" + BlueScore.ToString() + ")B v. (" + PurpleScore.ToString() + ")P\n");
            m_PurpleAgentGroup.AddGroupReward(1);
            m_BlueAgentGroup.AddGroupReward(-1);
        }
        //m_PurpleAgentGroup.EndGroupEpisode();
        //m_BlueAgentGroup.EndGroupEpisode();
        //ResetScene();
        ResetBall();

    }

    public void GiveReward()
    {
        
    }


    public void ResetScene()
    {
        m_ResetTimer = 0;

        //Reset Agents
        foreach (var item in AgentsList)
        {
            var randomPosX = Random.Range(-2f, 2f);
            var newStartPos = item.Agent.initialPos + new Vector3(randomPosX, 0f, 0f);
            var rot = item.Agent.rotSign * Random.Range(80.0f, 100.0f);
            var newRot = Quaternion.Euler(0, rot, 0);
            item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);

            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
        }
        //Reset Score
        BlueScore = 0;
        PurpleScore = 0;
        //Reset Ball
        ResetBall();
    }

    public Paradime GetParadime()
    {
        return paradime;
    }
}
