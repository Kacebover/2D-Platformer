using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected int lives;
    [SerializeField] protected AudioSource mondamageSound;

    public virtual void GetDamage()
    {
        if (lives > 0)
        {
            mondamageSound.Play();
            lives--;
            if (lives == 0)
                Die();  
        }
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }
}
