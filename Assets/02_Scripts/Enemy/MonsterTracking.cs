using UnityEngine;

public class MonsterTracking : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public float range = 20f;

    public float chaseDelay = 200.0f;
    public float chaseLimitDistance = 2.0f;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= range)
        {
            Vector3 playerPositionFlattened = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(playerPositionFlattened);

            if (distance > chaseLimitDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
        }
    }
}
