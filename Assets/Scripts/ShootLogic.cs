using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLogic : MonoBehaviour
{
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Transform camProxy;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private float gunDamage = 33.33f;

    public void Shoot(bool shootPressed, bool isAimed)
    {
        if (shootPressed)
        {
            //ShootBullet();
            print("ShootPressed" + gunDamage);
        }

        if (isAimed)
        {
            print("Aiming");
        }
    }

    //private void ShootBullet()
    //{
    //    if(Physics.Raycast(bulletSpawn.position, camProxy.forward, out RaycastHit hit, 10000, enemyLayer))
    //    {
    //        Player enemy = hit.collider.gameObject.GetComponent<Player>();
    //        print(enemy);
    //        enemy.health -= (int)gunDamage;

    //        UpdateHealth(enemy.Id, enemy.health);
    //    }
    //}

    private void UpdateHealth(ushort id, int health)
    {
        Message message = Message.Create(MessageSendMode.Reliable, (ushort) ServerToClientId.updateHealth);
        message.AddUShort(id);
        message.AddInt(health);

        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
