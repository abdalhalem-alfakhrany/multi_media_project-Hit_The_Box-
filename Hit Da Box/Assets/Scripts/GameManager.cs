using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private System.Random _rand;
    private Camera _camera;
    
    public static GameManager Instance;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject boxesCollectionPrefab;
    
    // ui
    [SerializeField] private Text scoreText;
    [SerializeField] private Text heightsScoreText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text upgradesText;
    [SerializeField] private Text gameOverText;

    // boxes spawn
    private float _lastSpawnTimer;
    private float _spawnInterval = 3.0f;
    
    // machine gun upgrade
    private float _lastMachineGunThrowTimer;
    private float _machineGunTimer;
    private bool _machineGunUpgrade;
    
    // health
    private int _health;

    // score
    private int _score;
    private int _heightsScore;
    
    // game over
    private bool _gameOver;

    private void Awake() {
        _rand = new System.Random(); 
        _camera = Camera.main;
        
        Instance = this;        

        // ui initialization
        _heightsScore = PlayerPrefs.GetInt("heights_Score");
        heightsScoreText.text = $"Heights Score {_heightsScore}";
        
        _health = 3;
        healthText.text = $"Health : {_health}";
    }
    
    private void Update() {
        
        if (_gameOver) 
            return;
            
        if (_lastSpawnTimer >= _spawnInterval) {
            SpawnBoxesRandomly();
            _lastSpawnTimer = 0;
        }
        else {
            _lastSpawnTimer += Time.deltaTime;
        }

        if (_machineGunUpgrade) {
            if(Input.GetMouseButton(0)) {
                if (_lastMachineGunThrowTimer > 0.2f) {
                    ThrowBall();
                    _lastMachineGunThrowTimer = 0;
                }
                else {
                    _lastMachineGunThrowTimer += Time.deltaTime;
                }
                _machineGunTimer += Time.deltaTime;
            }

            upgradesText.text = $"upgrades : Machine Gun {(int)_machineGunTimer}";

            if (!(_machineGunTimer >= 3.0f)) return;
            
            upgradesText.text = "upgrades : 0";
            _machineGunUpgrade = false;
            
        }else {
            if (Input.GetMouseButtonDown(0)) 
                    ThrowBall();
        }
    }

    private void OnDestroy() {
        PlayerPrefs.SetInt("heights_Score",Mathf.Max(_score, _heightsScore));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void ThrowBall() {
        var proj = Instantiate(projectilePrefab , _camera.transform.position, Quaternion.Euler(Vector3.zero));
       
        var mousePos = Input.mousePosition;
        mousePos.z = _camera.nearClipPlane + 1;
        var ray = _camera.ScreenPointToRay(mousePos);
        
        if (!new Plane(Vector3.up, 0).Raycast(ray, out var distance)) return;
        
        var velocity = (ray.GetPoint(distance) - _camera.transform.position).normalized * 30;
        proj.GetComponent<Rigidbody>().velocity = velocity ;
    }

    private void SpawnBoxesRandomly() {
        var parentPosition = new Vector3(Mathf.Clamp(_rand.Next(-5, 5),-4.5f,4.5f), 1.5f , Mathf.Clamp(_rand.Next(-5, 5),-4.5f,4.5f));
        Instantiate(boxesCollectionPrefab,parentPosition,Quaternion.Euler(Vector3.zero)) ;
    }

    public void IncreaseScore() {
        switch (_score) {
            case 5:
                _spawnInterval = 2;
                break;
            case 10:
                _spawnInterval = 1;
                _machineGunUpgrade = true;
                break;
            case 20:
                _machineGunUpgrade = true;
                break;
        }
        
        _score++;
        scoreText.text = $"Score {_score}";
    }

    public void DecreaseHealth() {
        _health -= 1;
        healthText.text = $"Health {_health}";

        if (_health != 0) return;
        
        _gameOver = true;
        gameOverText.text = "Game Over";
    }
}
