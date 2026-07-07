using UnityEngine;

public class Catch : MonoBehaviour
{
    public EnemyController enemy;

    private bool hasCaught;//抓住期间不再触发

  

    private void OnTriggerStay2D(Collider2D other)
    {
        if (enemy == null) return;



        if (hasCaught) return;
        if (enemy.isHurt) return;
        if (enemy.isDead) return;
        if (enemy.isCatching) return;
        if (Time.time < enemy.nextCatchTime) return;

        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.isDead || player.isHurt || player.isCaptured) return;

        hasCaught = true;
        enemy.StartCatchPlayer(player);
    }

    public void ResetCatch()
    {
        hasCaught = false;
    }



   


}