using UnityEngine;

namespace StarterAssets
{
    public class ZombieHealth : MonoBehaviour
    {
        public float MaxHealth = 100f;
        private float _currentHealth;

        private ZombieAI _zombieAI;

        private void Awake()
        {
            _zombieAI = GetComponent<ZombieAI>();
        }

        private void OnEnable()
        {
            _currentHealth = MaxHealth;
        }

        public void TakeDamage(float damageAmount)
        {
            // Si ya está muerto, ignoramos los tiros extra (evita bugs de re-muerte)
            if (_currentHealth <= 0) return;

            _currentHealth -= damageAmount;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // ? BORRÁ ESTA LÍNEA SI LA TENÍAS:
            // Destroy(gameObject);

            //  LA FORMA CORRECTA: Le avisamos al script optimizado de IA que se encargue
            if (_zombieAI != null)
            {
                _zombieAI.TakeDamage();
            }
            else
            {
                // Plan B por si probás un zombie viejo que no tenga el script ZombieAI
                Destroy(gameObject);
            }
        }
    }
}