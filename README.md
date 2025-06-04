# 🎮 9to9in9

9to9in9 3D 서바이벌 프로젝트는 기능별 모듈화를 기반으로 유지보수와 확장성을 고려해 구성된 Unity 게임 스크립트 구조입니다.

<img width="1037" alt="image" src="https://github.com/user-attachments/assets/cffa1234-8984-4621-82e4-4f10bd0939e4" />
<img width="718" alt="image" src="https://github.com/user-attachments/assets/814ee60f-7ddb-4c0e-a230-c47d01b9b045" />
<img width="719" alt="image" src="https://github.com/user-attachments/assets/2050f9e0-397f-4e04-a8de-ac7d6971490a" />
<img width="719" alt="image" src="https://github.com/user-attachments/assets/c6d7b9ee-c84b-46e4-a412-e7ab575d6cc5" />
<img width="719" alt="image" src="https://github.com/user-attachments/assets/a9b717c0-8bd1-4d74-9575-ba3e8cfb55cb" />
<img width="722" alt="image" src="https://github.com/user-attachments/assets/895d3bfd-5085-424a-a430-c037d7a7093d" />
<img width="722" alt="image" src="https://github.com/user-attachments/assets/f520b0eb-f0ed-4c0a-8e23-944caa270b3e" />


---

## ⌚ 개발 기간
2025.05.26 ~ 2025.06.02

---

## 📁 01.Scripts/

### 🧠 Entity/
게임 내 동적 존재들(플레이어, 적 등)에 대한 핵심 로직을 포함합니다.

- `DayAndNight/`  
  - 낮과 밤 사이클(`DayNightCycle.cs`) 및 반딧불이(`Firefly.cs`) 처리
- `Enemy/`  
  - 적의 AI 및 상태 관리  
  - `BehaviorTree/`: 노드 기반 AI 구성 (Selector, Sequence, Action 등)
- `Player/`  
  - 플레이어 입력, 제어, 상호작용, 장비 및 인벤토리 시스템 처리
- `Stat/`  
  - 캐릭터 능력치 시스템 (Stat, StatProfile, StatHandler 등)

---

### 🧩 Interfaces/
게임 내 객체들의 기능 정의를 위한 인터페이스 모음입니다.

- `Entity/`:  
  - `IMoveable`, `IAttackable`, `IDamagable`, `IJumpable` 등
- `Item/`:  
  - `IInspectable`, `IInteractable`

---

### 🎒 Item/
아이템 시스템 전반을 담당합니다.

- `Components/`:  
  - 아이템 상호작용 및 행동 구현 (ex: `InteractableBehaviour`, `WeaponHandler`)
- `Craft/`:  
  - 제작 시스템 관련 로직 (레시피 핸들링, 로딩)
- `Data/`:  
  - SO 기반 아이템 정의 (`ItemData`, `BuildItemData`, `ResourceItemData` 등)
- `Manager/`:  
  - 아이템 및 제작, 리소스, 스폰 등 관련 매니저 클래스

---

### 🧰 Static/
- `StringExtensions.cs`: 문자열 관련 확장 메서드
- `StringNamespace.cs`: 아이템, 레시피 등에 사용되는 고정 문자열 상수 정의

---

### 🎮 UI/
게임 내 UI 전체를 구성하고 제어합니다.

- `Canvas/`:  
  - 메인 씬, 스타트 씬, 옵션 등의 UI 제어 스크립트
- `Dialogues/`:  
  - NPC 대화 시스템 처리
- `MainScene/GUI/`:  
  - 작업대, 인벤토리, 퀵슬롯, 상태창 등 메인 UI 구성
- `Extension/`:  
  - `AsyncOperation` 관련 유틸리티
- `Util/`:  
  - CSV 데이터 임포터 등 기타 도구

---

## 🧩 핵심 매니저 클래스

- `GameManager.cs`: 게임 상태 및 전역 흐름 관리
- `SoundManager.cs`: BGM/SFX 재생 및 볼륨 제어
- `UIManager.cs`: UI 전반 컨트롤
- `PlayerManager.cs`, `BuildManager.cs`, `DialogueManager.cs` 등: 각 기능별 전용 매니저

---

## ⚙️ 주요 시스템 예시

### 📦 아이템 시스템
- `ItemData` 기반 ScriptableObject 구조
- `InteractableBehaviour`로 상호작용 정의
- `ItemObject` → `InventoryController`로 연결

### 🔨 제작 시스템
- `RecipeLoader`: JSON 기반 레시피 로딩
- `CraftManager`: 아이템 제작 로직 실행
- `WorkstationType` 기반 제작소 구분 (`Workbench`, `Campfire`, `Anvil` 등)

### 🧠 AI 시스템
- `BehaviorTree` 폴더 내 노드 기반 적 행동 설계
- `EnemyController`에서 BT 실행 관리

---

## 📌 정리

저희 프로젝트 구조는 유연한 기능 확장을 위한 **기능 단위 모듈 구성**을 중심으로 설계했습니다.
특히 **아이템과 제작 시스템**, **플레이어-적 상호작용**, **UI 팝업 구조** 등은 명확한 책임 분리 원칙에 기반하여 구현되어 있습니다.

---

## 🔧 개발 환경 및 사용 패키지
- Unity 2022.3.17f1
- URP 사용 여부: X
- 사용된 주요 Unity 패키지:
  - `Input System`
  - `Addressables`
  - `DOTween`
  - `Newtonsoft.Json`
 
---

## 👪 Team Members

- 송규민(팀장) - 적, 전투 담당
- 고윤아 - 건축 담당
- 권윤원 - Player 및 인벤토리 담당
- 조아영 - 아이템 및 제작 담당
- 최영임 - UI 담당

## 💿 시연영상

https://www.youtube.com/watch?v=tiTJEwDb55s

