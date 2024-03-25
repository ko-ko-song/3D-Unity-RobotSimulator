import socket
import json

def create_message(choice):
    doorID = 'front_door1'
    evID='ev-204-1'
    if choice == 1:
        return {
            "id": doorID,
            "mType": "openDoor"
        }
    elif choice == 2:
        return {
            "id": doorID,
            "mType": "closeDoor"
        }
    elif choice == 3:
        floor = input("Enter floor: ")
        return {
            "id": evID,
            "mType": "openElevatorDoor",
            "floor": floor
        }
    elif choice == 4:
        floor = input("Enter floor: ")
        return {
            "id": evID,
            "mType": "closeElevatorDoor",
            "floor": floor
        }
    elif choice == 5:
        floor = input("Enter floor: ")
        return {
            "id": evID,
            "mType": "moveElevator",
            "floor": floor
        }
    else:
        print("Invalid choice.")
        return None

# 서버 정보 설정
# HOST = '192.168.0.58'  # 서버 주소
HOST = '192.168.0.161'  # 서버 주소
    
PORT = 20808        # 포트 번호

# TCP 소켓 생성 및 연결
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.connect((HOST, PORT))

    while True:
        try:
            choice = int(input("Enter your choice (1-5): "))
            message = create_message(choice)
            if message:
                json_message = json.dumps(message)
                s.send(json_message.encode('utf-8'))
                s.send(b'\n')  # Corrected line to send newline as bytes
                print(f"Sent message: {json_message}")
        except ValueError:
            print("Invalid input. Please enter a number.")

# 연결 종료 후 클라이언트 종료
print("Connection closed.")
