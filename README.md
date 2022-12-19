# UIManager

이 프로젝트는 Task 기반의 UIManager를 제작하기 위해 만들어졌습니다. **resource, server등의 여러 비동기 API에 대응하기 위해 비동기 Task를 통합 관리**하여 Page, Popup등의 UI를 띄우도록 설계되었습니다. 

Page 관리하는 모습

![Honeycam 2022-12-19 18-25-07.gif](UIManager%20cae58639c2734c54b806d95fe77d3929/Honeycam_2022-12-19_18-25-07.gif)

Popup 관리하는 모습 

![Honeycam 2022-12-19 18-24-04.gif](UIManager%20cae58639c2734c54b806d95fe77d3929/Honeycam_2022-12-19_18-24-04.gif)

### 앞서서

- 유니티에서 비동기 사용을 효율적으로 진행하기 위하여 [UniTask](https://github.com/Cysharp/UniTask)를 사용했습니다. 해당 내용은 [UniTask](https://github.com/Cysharp/UniTask)를 참조.
- 해당프로젝트는 Unity 2021.3, UniTask Ver.2.3.3에서 테스트 되었습니다.
- namespace 등에 포함된 회사명 ChickenGames는 현존하는 회사가 아니며, 만약 다른 곳에서 사용하고 있다면 저와 무관합니다.

## Getting Start

[Release](https://github.com/JongSeonPark/Unity_UIManager/releases)에서 관련 코드 예제를 다운 받을 수 있습니다.

### UIManager Instance 생성

사용하기 전에 Instance를 생성해주세요. 매개변수로는 IResourceLoader, uiRoot로 구성되어 있습니다. uiRoot는 UI의 부모가 될 transform을 넣어주시면 됩니다.

```csharp
public class UI_TestMain : MonoBehaviour
{
        [SerializeField]
        Transform uiRoot;

        void Start()
        {
            UIManager.CreateInstance(new UnityResourceLoader(), uiRoot);
        }
}
```

### ResourceLoader

```csharp
public interface IResourceLoader
    {
        UniTask<Object> LoadAsync(string path);
        UniTask<Object> InstantiateAsync(string path, Transform parent = null, bool instantiateInWorldSpace = false);
        bool ReleaseInstance(GameObject instance);
    }
```

IResourceLoader는 리소스 로드 방법을 지정하기 위한 인터페이스입니다. 유니티는 환경에 따라 리소스 로드 방식이 달라질 수 있습니다. 

- Resources
- AssetBundle
- Adressable

때문에 UIManager의 인스턴스를 생성때 지정하기 위해 작성되었습니다. 그렇기에 사용자가 구현해 줘야 합니다. 아래는 Unity의 Resources를 기준으로 작성한 코드입니다. Example에서 코드를 확인할 수 있습니다.

```csharp
public class UnityResourceLoader : IResourceLoader
    {
        public async UniTask<Object> InstantiateAsync(string path, Transform parent = null, bool instantiateInWorldSpace = false)
        {
            var objRes = await Resources.LoadAsync(path);

            if (objRes == null)
            {
                Debug.LogError($"{path} is not exist..");
                return null;
            }

            return Object.Instantiate(objRes, parent, instantiateInWorldSpace);
        }

        public async UniTask<Object> LoadAsync(string path)
        {
            return await Resources.LoadAsync(path);
        }

        public bool ReleaseInstance(GameObject instance)
        {
            Object.Destroy(instance);
            return true;
        }
    }
```

### Page 생성

PageBase를 상속받은 Component를 작성해주세요. 그리고 아래와 같이 Init를 작성합니다. 

LoadFuncAsyncs에서 Task를 추가합니다. 해당 페이지는 LoadFuncAsyncs가 완료되지 않는 한 열리지 않습니다.

```csharp
public class TestMainPage : PageBase
    {
        [SerializeField]
        Button openSubPageButton;

        public override void Init()
        {
            base.Init();

            LoadFuncAsyncs.Add(async (p, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                await UniTask.Delay(500);
                Debug.Log("Done1");
            });
            LoadFuncAsyncs.Add(async (p, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                await UniTask.Delay(1000);
                Debug.Log("Done2");
            });
            
            openSubPageButton?.onClick.AddListener(() =>
            {
                UIManager.Instance.OpenPage("TestSubPage");
            });
        }
    }
```

최상단에 스크립트를 포함한 Prefab을 제작합니다.

![화면 캡처 2022-12-19 183317.png](UIManager%20cae58639c2734c54b806d95fe77d3929/%25ED%2599%2594%25EB%25A9%25B4_%25EC%25BA%25A1%25EC%25B2%2598_2022-12-19_183317.png)

UIManager.Instance.OpenPage를 통해 Page를 로드합니다. 매개변수는 다음과 같습니다.

- path는 ResourceLoader에서의 Load path입니다. ResourceLoader에서 지정한 방식으로 리소스를 로드합니다.
- 필요시 IProgress<float> progress를 통해 진행도를 알 수 있습니다.
- CancellationToken cancellationToken는 중간에 생성 Task를 취소하기 위한 토큰입니다.
- int delay는 여는게 늘어졌을 때 딜레이 이벤트를 실행할 시간입니다. 딜레이 이벤트는 UIInstantiateRequest를 통해 발생시킬 수 있습니다. 아래에서 언급하겠습니다.

```csharp
public UIInstantiateRequest OpenPage(string path, IProgress<float> progress = null, CancellationToken cancellationToken = default, int delay = 1000)
```

사용 예시

```csharp
UIManager.Instance.OpenPage("TestSubPage");
```

### UIInstantiateRequest

UIManager를 통해 UI를 생성시에 UIInstantiateRequest를 반환합니다. UIInstantiateRequest에서 포함된 내용은 아래와 같습니다.

- onLoadingDelay: 지정한 시간보다 딜레이 되었을 때 발생할 이벤트를 등록
- onLoadingDelayDone: 만약 딜레이 되었을 때, 딜레이가 끝날 때 발생할 이벤트를 등록
- Complete: 로드 완료시에 발생할 이벤트를 등록

```csharp
async UniTask StartAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            openUICTS = new CancellationTokenSource();
            UIInstantiateRequest request = UIManager.Instance.OpenPopup("TestLoopPopup", cancellationToken: openUICTS.Token);
						
            // 딜레이 되었으니 로딩 UI를 생성함.
            request.onLoadingDelay.AddListener(() =>
            {
                loadingCTS = new CancellationTokenSource();
                UniTask.Void(async () =>
                {
                    var obj = await Resources.LoadAsync<GameObject>("LoadingUI");
                    loadingCTS.Token.ThrowIfCancellationRequested();
                    loadingObject = Instantiate(obj as GameObject);
                    loadingObject.GetComponent<Transform>().SetParent(panelTransform, false);
                });
            });

            // 딜레이가 끝났으니 생성된 로딩 UI를 제거함.
            request.onLoadingDelayDone.AddListener(() =>
            {
                loadingCTS.Cancel();
                loadingCTS = null;
                Destroy(loadingObject);
                loadingObject = null;
            });

            // 생성이 완료 되었을 때 위치를 잡음.
            request.Complete += (obj) =>
            {
                obj.GetComponent<Transform>().localPosition = transform.localPosition + new Vector3(10, 0);
            };
            await request;

            openUICTS?.Dispose();
            openUICTS = null;
        }
```

## 설치


이 프로젝트는 [UniTask](https://github.com/Cysharp/UniTask)를 사용하고 있습니다. 설치를 진행하기 전, 유니티 프로젝트에 [UniTask](https://github.com/Cysharp/UniTask)를 설치 후 진행해주세요. 

### git URL로 설치

![Untitled](UIManager%20cae58639c2734c54b806d95fe77d3929/Untitled.png)

![Untitled](UIManager%20cae58639c2734c54b806d95fe77d3929/Untitled%201.png)

Package Manager에서 `https://github.com/JongSeonPark/Unity_UIManager.git?path=Assets/UIManager`를 기입하여 추가할 수 있습니다. 

### openUPM으로 설치

[openUPM](https://openupm.com/)에 작성되어 있습니다. [openupm-cli](https://github.com/openupm/openupm-cli)를 이용해 설치할 수 있습니다.

```
openupm add com.chickengames.uimanager
```

### Packages/manifest.json에 추가하여 설치

Packages/manifest.json에서 아래 내용을 추가하여 설치할 수 있습니다.

`"com.chickengames.uimanager": "https://github.com/JongSeonPark/Unity_UIManager.git?path=Assets/UIManager"`

### 코드를 클론 후 사용

코드 수정 및 직접 제어를 원한다면 Git을 클론하여 [UIManager폴더 부분](https://github.com/JongSeonPark/Unity_UIManager/tree/main/Assets/UIManager)을 사용해도 무방합니다.

## 🍻 License


이 프로젝트는 Unity외의 외부에서 작성된 코드를 포함하고 있지 않으며, 제가 작성된 코드에 대해서는 Beer License를 사용하고 있습니다. 행복하세요. :)