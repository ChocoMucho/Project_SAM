using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using Unity.AI.Navigation;

public enum Enemy
{
    Turret,
    Puncher,
}

public class MainGateScene : BaseScene
{
    // 문 열림 -> 스폰 -> 문 닫힘
    // 이걸 위해서는 플레이어만 못지나가는 배리어가 필요함
    // 그리고 문 닫히고 나서는 배리어 없애야 함
    // 여기서 적 전부 처치 확인
    // 문 열림 작동

    // 플레이어 참조
    [SerializeField] private GameObject _player;

    // 적 프리팹들
    [SerializeField] private GameObject[] _enemyFrefabs;
    // 안보이는 플레이어용 바리게이트
    [SerializeField] private GameObject _barricade;
    [SerializeField] private GameObject _mainGate;
    // AI
    [SerializeField] private NavMeshSurface NavMeshSurface;


    private float _spawnCount;
    private float _killCount;
    private bool _spawnEnd = false;
    private Queue<Vector3> spawnPoints = new Queue<Vector3>();

    protected override void Init()
    {
        base.Init();
        SceneType = Scene.MainGate;

        // UI
        UIManager.Instance.Regist("UI_PlayerStatus");
        UIManager.Instance.Show("UI_PlayerStatus");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _barricade.SetActive(true);

        MakeSpawnPoints(new Vector3(0,0.1f,0), 10);

        StartCoroutine(SpawnRoutine());
    }
    public override void Clear()
    {}

    private void Update()
    {
        AllKillCheck();
    }

    private void IncreaseKillCount()
    {
        ++_killCount;
        Debug.Log($"Kill Count: {_killCount}");
    }

    private void AllKillCheck()
    {
        if(_spawnEnd)
        {
            //스폰 카운트랑 킬 카운트 비교
            if(_killCount >= _spawnCount)
            {
                _mainGate.GetComponent<Gate>().OpenTheGate();
                _barricade.SetActive(false);
            }
        }
    }

    private void MakeSpawnPoints(Vector3 center, int range)
    {
        // 중심에서 -range부터 +range까지 만들고 섞고 큐에 넣어주기
        List<Vector3> points = new List<Vector3>();
        for(int i = -range; i <= range; ++i)
        {
            for(int j = -range; j <= range; ++j)
            {
                float x = center.x + i;
                float z = center.z + j;
                points.Add(new Vector3(x, center.y, z));
            }
        }
        Utils.ShuffleList(points);
        spawnPoints = new Queue<Vector3>(points);
    }

    IEnumerator SpawnRoutine() // 얘 호출되면 스포닝 시작
    {
        SpwanEnemy(Enemy.Turret, 1);
        //SpwanEnemy(Enemy.Puncher, 1);
        yield return new WaitForSeconds(3f);

        _spawnEnd = true;
    }

    // 1. 스폰 카운트 증가 2. 플레이어 참조시키기
    private void SpwanEnemy(Enemy enemy, int count = 1)
    {
        Vector3 spawnPoint;
        for (int i = 0; i < count; ++i)
        {
            spawnPoint = spawnPoints.Dequeue();
            spawnPoints.Enqueue(spawnPoint);
            GameObject go = Instantiate(_enemyFrefabs[(int)enemy], spawnPoint, Quaternion.identity);
            
            if (enemy == Enemy.Turret) // enemy 상속으로 완전 개선 필요
            {
                go.GetComponent<Turret>().Target = _player;
                go.GetComponent<Turret>().OnDeath += IncreaseKillCount;
            }
            else
            {
                go.GetComponent<Puncher>().Target = _player;
                go.GetComponent<Puncher>().OnDeath += IncreaseKillCount;
            }

            // 스폰 카운트 증가
            ++_spawnCount;
        }
        NavMeshSurface.BuildNavMesh();
    }
}
