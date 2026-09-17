# SimBar - Lofi Cozy Room Pub Game

## 프로젝트 개요
- **장르**: 소규모 라이브 콘서트 펍/바 시뮬레이션
- **뷰**: 아이솔레이트 뷰 (lofi cozy room 스타일)
- **핵심 로직**: 손님 받기, 주문 처리, 음악 재생, 칵테일/술 제공

## 폴더 구조
```
Assets/
  Scripts/
    Game/
      GameManager.cs      - 전역 게임 상태, 돈, 평판
      OrderManager.cs     - 주문 수락 및 처리
      GameLoader.cs       - 씬 로드 및 싱글톤 초기화
    Customer/
      Customer.cs         - 손님 AI, 상태머신
      CustomerSpawner.cs  - 손님 생성 및 테이블 배정
    Bar/
      BarCounter.cs       - 바 카운터 상호작용
      Table.cs            - 테이블 좌석 상태
      Stage.cs            - 공연 스테이지, BGM 재생
      PlayerController.cs - 플레이어 이동 및 상호작용
    Audio/
      AudioManager.cs     - BGM/SFX 관리
    UI/
      UIManager.cs        - HUD, 메뉴
  Data/
    DrinkData.cs         - 마실것 데이터 (ScriptableObject)
    Order.cs             - 주문 데이터
  Audio/
    BGM/                 - 배경음악
    SFX/                 - 효과음
  Prefabs/
    (Customer, Player 등)
```

## 게임플레이 흐름
1. **GameLoader**가 시작 시 GameManager와 AudioManager를 생성
2. **CustomerSpawner**가 주기적으로 손님을 생성
3. 손님은 빈 **Table**에 앉아 **OrderManager**를 통해 주문
4. 플레이어는 바 카운터에서 `E` 키를 눌러 주문 처리
5. **Stage**에서 Guitar/Singing 모델이 공연하며 BGM 재생
6. 손님을 만족시키면 돈과 평판 상승

## 씬 구성 가이드
1. **MainScene** (SampleScene.unity 사용)
   - 카메라: Isometric 또는 Top-Down View
   - 조명: Warm Point Lights, Ambient Color (Cozy tone)
   - 바 카운터: kenney_mini-market의 `cash-register`, `shelves`, `freezer` 사용
   - 테이블: kenney_mini-arcade의 테이블/의자 활용
   - 스테이지: `Model/Guitar Playing.fbx`와 `Model/Singing.fbx` 배치
   - 배경: Isometric 카메라로 전체 바를 한눈에 보이도록 배치

2. **Prefab 생성**
   - Customer: 빈 GameObject에 Customer.cs + NavMeshAgent + Animator + CapsuleCollider 추가
   - Player: PlayerController.cs + CharacterController 추가
   - BarCounter: BarCounter.cs + 여러 ServicePoint 자식 배치

3. **NavMesh Bake**
   - 바닥과 계단에 NavMesh Surface 컴포넌트 추가
   - Bake 실행

4. **Input System**
   - InputSystem_Actions.inputactions 파일이 이미 존재함
   - Player 오브젝트에 PlayerInput 컴포넌트 추가 후 Actions 할당

5. **Audio 설정**
   - Assets/Audio/BGM 폴더에 lofi BGM 넣기
   - Assets/Audio/SFX 폴더에 효과음 넣기

## 다음 단계
- [ ] DrinkData ScriptableObject 생성 (칵테일, 맥주, 위스키 등)
- [ ] Customer 프리팹 생성 및 애니메이션 연결
- [ ] UI Canvas 구성 (주문 패널, 돈 표시)
- [ ] 실제 BGM/SFX 에셋 추가
- [ ] 모델 임포트 및 LOD 설정
