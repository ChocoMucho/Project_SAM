using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using Unity.AI.Navigation;

public enum EnemyType
{
    Turret,
    Puncher,
}

public class MainGateScene : BaseScene
{
    // �� ���� -> ���� -> �� ����
    // �̰� ���ؼ��� �÷��̾ ���������� �踮� �ʿ���
    // �׸��� �� ������ ������ �踮�� ���־� ��
    // ���⼭ �� ���� óġ Ȯ��
    // �� ���� �۵�

    // �÷��̾� ����
    [SerializeField] private GameObject _player;

    // �� �����յ�
    [SerializeField] private GameObject[] _enemyFrefabs;
    // �Ⱥ��̴� �÷��̾�� �ٸ�����Ʈ
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
            //���� ī��Ʈ�� ų ī��Ʈ ��
            if(_killCount >= _spawnCount)
            {
                _mainGate.GetComponent<Gate>().OpenTheGate();
                _barricade.SetActive(false);
            }
        }
    }

    private void MakeSpawnPoints(Vector3 center, int range)
    {
        // �߽ɿ��� -range���� +range���� ����� ���� ť�� �־��ֱ�
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

    IEnumerator SpawnRoutine() // �� ȣ��Ǹ� ������ ����
    {
        SpwanEnemy(EnemyType.Turret, 1);
        //SpwanEnemy(Enemy.Puncher, 1);
        yield return new WaitForSeconds(3f);

        _spawnEnd = true;
    }

    // 1. ���� ī��Ʈ ���� 2. �÷��̾� ������Ű��
    private void SpwanEnemy(EnemyType enemy, int count = 1)
    {
        Vector3 spawnPoint;
        for (int i = 0; i < count; ++i)
        {
            spawnPoint = spawnPoints.Dequeue();
            spawnPoints.Enqueue(spawnPoint);
            GameObject go = Instantiate(_enemyFrefabs[(int)enemy], spawnPoint, Quaternion.identity);

            go.GetComponent<Enemy>().Target = _player;
            go.GetComponent<Turret>().OnDeath += IncreaseKillCount;

            /*if (enemy == EnemyType.Turret) // enemy ������� ���� ���� �ʿ�
            {
                
            }
            else
            {
                go.GetComponent<Puncher>().Target = _player;
                go.GetComponent<Puncher>().OnDeath += IncreaseKillCount;
            }*/

            // ���� ī��Ʈ ����
            ++_spawnCount;
        }
        NavMeshSurface.BuildNavMesh();
    }
}
