using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject _boxesParent;
    private bool _isCollided;
    
    private void Update() {
        if (!_isCollided && (transform.position.x >= 5 || transform.position.z >= 5))
        {
            GameManager.Instance.DecreaseHealth();   
            Destroy(gameObject);
        }
    }

    private void Destroy() {
        Destroy(_boxesParent);
        Destroy(gameObject);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.name.Contains("Box") || _isCollided) return;
        _isCollided = true;
        _boxesParent = collision.transform.parent.gameObject;
        Invoke(nameof(Destroy),0.2f);
        GameManager.Instance.IncreaseScore();
    }
}
