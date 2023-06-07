using UnityEngine;

public class NoteMovement : MonoBehaviour
{
    private float speed;

    public void Initialize(float bpm)
    {
        // 1 beat = 60 / bpm seconds
        // 1 measure (4 beats in 4/4 time) = 4 * (60 / bpm) = 240 / bpm seconds
        // We want 1 measure to last 2 seconds, so we need to multiply by a factor of 2 / (240 / bpm) = bpm / 120.
        speed = bpm / 120f;
    }

    private void Update()
    {
        transform.position += new Vector3(-speed * Time.deltaTime * 5, 0, 0);
    }
}