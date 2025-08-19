using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceRespawnController : MonoBehaviour
{
    [Header("자원 목록")]
    [SerializeField] private List<GameObject> treePrefabs = new List<GameObject>();
    [SerializeField] private int maxTreeCnt = 5; // 소환 가능한 최대 나무 개수
    [SerializeField] private List<GameObject> rockPrefabs = new List<GameObject>();
    [SerializeField] private int maxRockCnt = 5;
    [SerializeField] private LayerMask groundLayer; // 땅 체크용
    [SerializeField] private float spawnRadius = 40;   // 리스폰 영역 반경(맵 크기)
    [SerializeField] private float minDistance = 3f;    // 기존 자원과 최소 거리
    [SerializeField] private float respownTime = 0.5f; // 리스폰 타임
    public List<ItemData> testInventory = new List<ItemData>(); // 테스트용

    private int curTreeCnt = 0;
    private int curRockCnt = 0;

    int maxAttempts = 20;  // 랜덤 소환 시도 횟수, 무한 루프 방지용
    private HashSet<Vector2> spawnedPos = new HashSet<Vector2>(); // x, z값만 저장하도록

    private void Start()
    {
        InvokeRepeating("Respawn", 0f, respownTime); // respownTime초마다 리스폰
    }
    private void Update()
    {
        // 테스트 용, 추후 플레이어 컨트롤로 이동
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("자원 캐기");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Interacterble", "Resource"))) // 리소스와 상호작용 가능한 것만 체크
            {
                Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), hit.point, Color.magenta, 1f);
                if(hit.collider.TryGetComponent(out Resource resource)) // 캘 수 있는 자원이라면
                {

                    // 추후 아래 부분 갈아 엎어야 함
                    if(resource.gameObject.CompareTag("Tree"))
                    {
                        Debug.Log("자원 캐기: 나무");

                        resource.Gather(hit.point, hit.normal, 1, ref curTreeCnt);
                    }
                    else if(resource.gameObject.CompareTag("Rock"))
                    {
                        Debug.Log("자원 캐기: 돌");
                        resource.Gather(hit.point, hit.normal, 1, ref curRockCnt);
                    }

                }
                else if(hit.collider.TryGetComponent(out IInteractable interactable)) // 얻을 수 있는 자원이라면
                {
                    Debug.Log("자원 획득");
                    //interactable.OnInteractTest(testInventory);
                    interactable.OnInteract();
                }
            }
        }
    }
    void Respawn()
    {
        // 나무를 더 소환할 수 있다면
        if(curTreeCnt < maxTreeCnt)
        {
            int prefabIdx = Random.Range(0, treePrefabs.Count);
            CheckRespawn(treePrefabs[prefabIdx], ref curTreeCnt);
        }
        // 돌
        if(curRockCnt < maxRockCnt)
        {
            int prefabIdx = Random.Range(0, rockPrefabs.Count);
            CheckRespawn(rockPrefabs[prefabIdx], ref curRockCnt);
        }
    }
    void CheckRespawn(GameObject prefab, ref int resouceCnt)
    {
        int tmpCnt = 0;
        while(tmpCnt++ < maxAttempts) // 소환을 최대한 하는 방향으로, while(true)라면 멈출 수도 있으니 최대 시도 횟수 설정
        {
            // 랜덤 좌표
            Vector3 randomPos = gameObject.transform.position + Random.insideUnitSphere.normalized * Random.Range(0f, spawnRadius);
            // x, z값만 저장해서 쓰기
            Vector2 randomPos2;
            randomPos2.x = randomPos.x;
            randomPos2.y = randomPos.z;

            // 중복된 좌표인가, 중복되었다면 다시 좌표 생성
            if (spawnedPos.Contains(randomPos2)) continue;

            // 기존 자원과의 거리 체크
            bool bCanSpawn = true;
            foreach(Vector2 pos in spawnedPos)
            {
                // 기존 물체와의 거리 체크하기
                if (Vector2.Distance(randomPos2, pos) > minDistance) continue;
                //Debug.Log($"{Vector2.Distance(randomPos2, pos)}, {tmpCnt}");
                bCanSpawn = false;
                break;
            }
            // 소환 불가라면 다음 좌표 생성
            if (!bCanSpawn) continue;
            // 기존 물체와 거리상의 문제가 없다면 땅 위 좌표 찾기
            // 땅이 없다면 다음 좌표 생성
            if (!Physics.Raycast(randomPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, groundLayer)) continue;

            Vector3 spawnPos = hit.point;
            // 랜덤 회전값 주기, y값만(yaw)
            float randomY = Random.Range(0f, 360f);
            Quaternion rot = Quaternion.Euler(0f, randomY, 0f);
            Instantiate(prefab, spawnPos, rot).transform.SetParent(gameObject.transform);
            spawnedPos.Add(new Vector2(spawnPos.x, spawnPos.z));
            resouceCnt++;
            //Debug.Log($"{prefab.name} 리스폰");
            break; // 소환 끝나면 while 종료
        }
        
    }

}
