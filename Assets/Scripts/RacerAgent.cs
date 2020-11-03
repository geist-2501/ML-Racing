using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RacerAgent : Agent
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float slowSpeed = 2f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float steeringSensitivity = 5f;

    private Rigidbody m_Rigidbody;
    private Pose m_StartingPose;
    private ISet<Checkpoint> m_PassedCheckpoints;
    private float m_TimeAlive;
    
    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        // Cache to prevent native code transition twice.
        var t = transform;
        m_StartingPose = new Pose(t.position, t.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        var checkpoint = other.GetComponent<Checkpoint>();
        
        if (checkpoint == null || m_PassedCheckpoints.Contains(checkpoint)) return;
        m_PassedCheckpoints.Add(checkpoint);
        
        var score = Vector3.Dot(checkpoint.transform.forward, transform.forward);
        
        if (!(score > 0)) return;
        
        var rewardModifier = maxSpeed / m_Rigidbody.velocity.magnitude;
        AddReward(1f * rewardModifier);
    }

    private void OnCollisionEnter(Collision other)
    {
        var track = other.gameObject.GetComponent<Track>();
        
        if (track == null) return;

        
        AddReward(-0.5f);
        EndEpisode();
    }

    public override void OnActionReceived(float[] vectorActions)
    {
        switch (vectorActions[0])
        {
            case 0: // Break.
                m_Rigidbody.velocity *= 1 / slowSpeed;
                break;
            case 1: // Accelerate.
                m_Rigidbody.AddRelativeForce(Vector3.forward * moveSpeed);
                if (m_Rigidbody.velocity.magnitude > maxSpeed)
                {
                    m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * maxSpeed;
                }
                break;
        }

        var steering = (vectorActions[1] - 1) * steeringSensitivity;
        
        transform.RotateAround(transform.position, Vector3.up, steering);
    }

    public override void OnEpisodeBegin()
    {
        transform.SetPositionAndRotation(m_StartingPose.position, m_StartingPose.rotation);
        m_PassedCheckpoints = new HashSet<Checkpoint>();
        
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_Rigidbody.velocity);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Vertical");
        actionsOut[1] = Input.GetAxis("Horizontal") + 1;
    }
}
