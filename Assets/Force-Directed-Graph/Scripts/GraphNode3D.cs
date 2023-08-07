using ForceDirectedGraph.DataStructure;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForceDirectedGraph
{
    public class GraphNode3D : MonoBehaviour
    {

        #region Constants

        /// <summary>
        /// The maximum value the node's velocity can be at any time.
        /// </summary>
        private const float MAX_VELOCITY_MAGNITUDE = 400f;

        #endregion

        #region Initialization

        /// <summary>
        /// Executes once on start.
        /// </summary>
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();

            // Freeze rotation
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.freezeRotation = true;
        }

        /// <summary>
        /// Initializes the graph entity.
        /// </summary>
        /// <param name="node">The node being presented.</param>
        public void Initialize(Node node)
        {
            _Node = node;
        }

        #endregion

        #region Fields/Properties

        /// <summary>
        /// The node being presented.
        /// </summary>
        [SerializeField]
        [Tooltip("The node being presented.")]
        private Node _Node;

        /// <summary>
        /// The node being presented.
        /// </summary>
        public Node Node { get { return _Node; } }


        /// <summary>
        /// References the rigid body that handles the movements of the node.
        /// </summary>
        private Rigidbody Rigidbody;


        /// <summary>
        /// List of all forces to apply.
        /// </summary>
        private List<Vector3> Forces;

        #endregion

        #region Movement

        /// <summary>
        /// Apply forces to the node.
        /// </summary>
        /// <param name="applyImmediately">States whether we should apply the forces immediately or wait till the next frame.</param>
        public void ApplyForces(List<Vector3> forces, bool applyImmediately = false)
        {
            if (applyImmediately)
                foreach (var force in forces)
                    Rigidbody.AddForce(force);
            else
                Forces = forces;
        }

        /// <summary>
        /// Updates the forces applied to the node.
        /// </summary>
        private void Update()
        {
             Rigidbody.velocity = Vector3.zero;

             Vector3 velocity = Vector3.zero;
             if (Forces != null)
                 foreach (var force in Forces)
                     velocity += force;

             velocity = velocity.normalized * Mathf.Clamp(velocity.magnitude, 0f, MAX_VELOCITY_MAGNITUDE);

             Rigidbody.AddForce(velocity);
        }

        #endregion

    }
}
