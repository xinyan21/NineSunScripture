import itertools as its
from random import choice

def countRepeat(pwd, char):
     cnt = 0
     for c in pwd:
        if c == char:
            cnt += 1
     return cnt

if __name__ == '__main__':
    words_num = "*ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    words_letter = 'abcdefghijklmnopqrstuvwxyz'
    #r = its.product(words_letter,repeat=8)
    dic = open("password-8位数字.txt","w")
    pwd = ''
    for a in range(26):
        pwd += words_letter[a]
        pwd += '*'
        for b in range(26):
            pwd += words_letter[b]
            for c in range(26):
                if countRepeat(pwd, words_letter[c]) > 2:
                    continue
                pwd += words_letter[c]
                for d in range(26):
                    if countRepeat(pwd, words_letter[d]) > 2:
                        continue
                    pwd += words_letter[d]
                    for e in range(26):
                        if countRepeat(pwd, words_letter[e]) > 2:
                            continue
                        pwd += words_letter[e]
                        for f in range(26):
                            if countRepeat(pwd, words_letter[f]) > 2:
                                continue
                            pwd += words_letter[f]
                            for g in range(26):
                                temp = pwd
                                if countRepeat(temp, words_letter[g]) > 2:
                                    continue
                                temp += words_letter[g]
                                dic.write("".join(temp))
                                dic.write("".join("\n"))
                            pwd = ''
   
    dic.close()
    print("完全写入成功")



#判断new的值是否在old之中
def judge_repeat(new, old):
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
    if lenth > len(passwd_lib):
        print('长度超出范围!!!')
        exit()
    tmp_pwd = ''
    password = ''
    for i in range(lenth):
        password = judge_repeat(tmp_pwd, password)
    return password
