from dataclasses import dataclass
from os import getlogin
import string
import requests

winUser = getlogin()
path = f'C:\\Users\\{winUser}\\.htl'

URL = 'http://10.10.0.251:8002/index.php?zone=cp_htl'
REDIURL = 'http://10.10.0.2/captiveportal/cp_logon_done.html'


@dataclass
class User:
    username: string
    password: string


def getUser():
    try:
        with open(path) as f:
            lines = f.readlines()
            return User(lines[0], lines[1])
    except:
        print("Irgendwos passt an deim file ned")
        return setUser()


def setUser():
    user = User(input("Username: "), input("Password: "))

    if (not user.username or not user.password):
        print("Invalid credentials\n")
        user = setUser()

    with open(path, 'w') as f:
        f.writelines([user.username, user.password])

    return user


user = getUser()

url = "http://10.10.0.251:8002/index.php?zone=cp_htl"

payload = f'auth_user={user.username}&auth_pass={user.password}&accept=Anmelden&rediurl={REDIURL}'
headers = {
    'Content-Type': 'application/x-www-form-urlencoded'
}

response = requests.request("POST", url, headers=headers, data=payload)
