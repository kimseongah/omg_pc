# 🏸OMG: Online Madminton Game

# 🚀 소개

> 🏸 Online Madminton Game, OMG로 같이 배드민턴 치자!


# 👩‍💻👨‍💻 팀원

- [김성아](https://www.notion.so/382c2e0466fd4d999ba930f53312be6a?pvs=21) : 한양대학교 컴퓨터소프트웨어학부 20학번
- [정성엽](https://www.notion.so/abcbf8ec1f234793800f7e0fd307ed5b?pvs=21)  : KAIST 전산학부 22학번

# ⚙️ 기술 스택

- 💻 PC - Unity
- 📱 Mobile - Flutter
- 🖥️ Server - Node js

# ℹ️ 기능 소개

## 주요 기능

- Online Madminton Game, OMG는 닌텐도 스위치, wii와 같은 별도의 장비를 구매하지 않고도 함께 배드민턴을 칠 수 있는 게임입니다.
- 휴대폰을 라켓처럼 이용해서 모니터 화면을 보며 배드민턴 게임을 할 수 있습니다.

## 1. 타이틀 화면

| ![title1](https://github.com/kimseongah/omg_pc/assets/71690205/7a7182c3-fcb8-4d54-9063-e50c6d2211a7) | ![title2](https://github.com/kimseongah/omg_pc/assets/71690205/6f4ab1f2-4a5e-4e68-89f5-0067ea15a7fe) |
| --- | --- |

### 1.1. 주요 기능

- CREATE 버튼을 선택 후 클릭하면 [2. 대기 화면](https://www.notion.so/2-97fe50035b28488fb676ed29b9529c0f?pvs=21) 으로 이동합니다.
- EXIT 버튼을 선택 후 클릭하면 게임이 종료됩니다.

## 2. 대기 화면

![ready1](https://github.com/kimseongah/omg_pc/assets/71690205/9fd08ee0-85bc-4325-bf18-fe067d834213)

| ![ready2](https://github.com/kimseongah/omg_pc/assets/71690205/cd752576-5ae6-49db-beb5-59977a2aa1dd) | ![ready3](https://github.com/kimseongah/omg_pc/assets/71690205/36497a3c-89ad-4cb3-9c37-336ec97fe5f5) |
| --- | --- |

### 2.1. 주요 기능

- 화면 하단에 고유한 코드가 표시됩니다.
- 휴대폰에서 자신의 닉네임과 해당 코드를 입력하여 참가하고, 다시 나갈 수 있습니다.
- 두 사람의 닉네임은 서로 같을 수 없으며, 두 명이 참가하게 되면 5초의 카운트다운 이후 [3. 게임 화면](https://www.notion.so/3-21fbd0b586644af5b2968016768a40a3?pvs=21)으로 이동합니다.

### 2.2. 기술 설명

- Client가 요청을 보내면 Server 측에서 현재까지 생성된 코드와 중복되지 않는 고유한 코드를 생성하여 응답을 보냅니다.
- socket.io를 이용해 사용자의 휴대폰, pc, 서버가 서로 실시간으로 통신합니다.

## 3. 게임 화면

![Untitled](https://github.com/kimseongah/omg_pc/assets/71690205/cf9ea3e1-7b00-4f55-83b9-cbaee03ed87c)

### 3.1. 주요 기능

- 게임의 배드민턴 라켓은 휴대폰의 센서와 연동돼있습니다.
- 휴대폰을 휘두르면 배드민턴 라켓이 움직여서 셔틀콕을 칠 수 있습니다.
- 처음 서비스 때는 콕이 공중에 있고 해당 콕을 상대편에게 보냅니다.
- 콕이 다가올 위치로 player가 이동됩니다.

### 3.2. 기술 설명

- 휴대폰의 자이로 센서 값이 socket.io를 이용해 서버에 실시간으로 보내지고, pc에서 그 값을 받아 게임 속 라켓을 휘두를 수 있습니다.
- 서브 할 때는 콕에 중력을 적용하지 않아 공중에 있을 수 있습니다.
- 콕은 포물선 운동을 하므로 라켓의 y 좌표와 같은 값이 되는 시간 t를 구해서 x, z 좌표를 계산합니다.
- 코루틴으로 player가 이동하도록 합니다.
