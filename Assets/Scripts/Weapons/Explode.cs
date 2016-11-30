using UnityEngine;
using System.Collections.Generic;


/**
 * Script that applies an explosion effect to any mesh by splitting the mesh
 * into individual triangles and animating the triangles using a particle-system.
 * 
 * Usage: The game-object that has this script applied must also have a 
 * ParticleSystem (shuriken) applied. The particle-system controls the behavior
 * of the explosion! 
 * For best results, the simulation-space of the particle system should be set to "World".
 * The renderer of the particle system should also be disabled, unless you want 
 * visible particles (is useful for testing the effect in the scene view).
 * 
 * Notes: This is a really basic approach to this and it doesn't scale well. Hi-poly meshes
 * produce a noticeable lag at the start of the explosion, as they are split into GameObjects.
 * This could be avoided by performing the splitting in the start method or using a completely 
 * different approach (vertex shaders?)
 * 
 * @author bummzack
 */
public class Explode : MonoBehaviour {

    public ParticleSystem ParticleTemplate;

    private ParticleSystem.Particle[] _particles;
    private Vector3[] _normals;
    private List<GameObject> _parts;
    private int _state;
    private Transform _transform;

    public void Start() {
        if (ParticleTemplate == null) {
            ParticleTemplate = GetComponent<ParticleSystem>();
        }

        if (ParticleTemplate == null) {
            Debug.LogError("Explode doesn't work without ParticleSystem");
            Destroy(this);
        }
        else {
            // don't emit any particles at start
            ParticleTemplate.Stop();
        }

        _transform = transform;
        _state = 0;
    }

    public void Update() {
        if (_state == 0) {
            return;
        }

        // destroy all shards when particle animation is complete
        if (!ParticleTemplate.IsAlive()) {
            _state = 0;
            foreach (GameObject obj in _parts) {
                Destroy(obj);
            }
            return;
        }

        // any attempt to set particles at an earlyer stage sadly gets ignored...
        if (_state == 2) {
            // read the particles from the system. This is important, because all attributes (lifetime, velocity, etc.)
            // will be set and must be copied
            ParticleTemplate.GetParticles(_particles);

            // set particle positions to shard positions
            for (int i = 0, len = _parts.Count; i < len; ++i) {
                _particles[i].position = _parts[i].transform.position;
            }
            ParticleTemplate.SetParticles(_particles, _parts.Count);
        }

        // at "frame" nr 3, we can start applying particle positions to our shards
        if (_state > 2) {
            int count = ParticleTemplate.GetParticles(_particles);

            for (int i = 0, len = _parts.Count; i < len; ++i) {
                if (i > count) {
                    break;
                }

                if (_particles[i].lifetime > 0.0f) {
                    _parts[i].transform.position = _particles[i].position;
                    Vector3 n = _normals[i];
                    _parts[i].transform.eulerAngles = n * _particles[i].rotation;
                }
                else {
                    _parts[i].GetComponent<Renderer>().enabled = false;
                }
            }
        }
        // modify state, because particles need some updates to get their initial positions and lifetime set :(
        if (_state <= 3) {
            _state++;
        }
    }

    // Play the explosion effect. This splits the object into triangles (each will become a separate
    // game object). Is very CPU intensive with hi-poly meshes.
    public void Play() {
        if (_state > 0) {
            return;
        }

        Mesh baseMesh = GetComponent<MeshFilter>().mesh;
        if (baseMesh == null) {
            return;
        }

        int triCount = baseMesh.triangles.Length / 3;
        _particles = new ParticleSystem.Particle[triCount];
        _parts = new List<GameObject>(triCount);
        _normals = new Vector3[triCount];

        for (int i = 0; i < triCount; ++i) {
            Mesh mesh = new Mesh();

            Vector3 v0 = baseMesh.vertices[baseMesh.triangles[i * 3]];
            Vector3 v1 = baseMesh.vertices[baseMesh.triangles[i * 3 + 1]];
            Vector3 v2 = baseMesh.vertices[baseMesh.triangles[i * 3 + 2]];
            Vector3 center = (v0 + v1 + v2) / 3.0f;

            // transform coordinates into world space
            v0 = _transform.TransformPoint(v0);
            v1 = _transform.TransformPoint(v1);
            v2 = _transform.TransformPoint(v2);
            center = _transform.TransformPoint(center);

            Vector3[] verts = new Vector3[]{
                v0 - center, v1 - center, v2 - center
            };

            Vector2[] uvs = new Vector2[]{
                baseMesh.uv[baseMesh.triangles[i * 3]],
                baseMesh.uv[baseMesh.triangles[i * 3 + 1]],
                baseMesh.uv[baseMesh.triangles[i * 3 + 2]]
            };

            Vector3[] normals = new Vector3[]{
                baseMesh.normals[baseMesh.triangles[i * 3]],
                baseMesh.normals[baseMesh.triangles[i * 3 + 1]],
                baseMesh.normals[baseMesh.triangles[i * 3 + 2]]
            };

            int[] tri = new int[] { 0, 1, 2 };

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tri;
            mesh.normals = normals;

            // create a perpendicular normal to use for rotation
            _normals[i] = Vector3.Cross(Vector3.Normalize(normals[0]), Vector3.Normalize(verts[1] - verts[0]));

            // create a new shard
            GameObject obj = new GameObject("Shard");
            obj.AddComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            obj.transform.position = center;
            obj.transform.parent = transform.parent;

            _particles[i].position = center;

            _parts.Add(obj);
        }


        ParticleTemplate.SetParticles(_particles, triCount);
        ParticleTemplate.Emit(triCount);
        ParticleTemplate.Stop();

        // mesh no longer needs to be rendered, as it has been replaced by shards
        GetComponent<Renderer>().enabled = false;
        _state = 1;
    }

}