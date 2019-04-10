# git操作

## git remote
git remote -v  查看本地分支关联远端分支路径,未关联为null。

* remote add
```
关联本地分支到远端git路径
git remote add  origin git@github.com:guccang/HexMap.git
```
* git remote -v

  ![tu](./git_pic/remote_01.bmp)

* git push origin master
```
  将本地分支同步到远端对应的master分支
```

* git push --set-upstream origin master
```
  将本地分支追踪
```


## git config

git --global user.name ""
git --global user.email ""
配置文件名称.gitconfig
```
[user]
        name = guccang@126.com
        email = guccang@126.com
[core]
        autocrlf = false
        excludesfile = C:/Users/.gitignore
```

多个git库如何配置config 文件. 路径 ~/.ssh下
~/.ssh/config
```
# github
Host github.com
HostName github.com
PreferredAuthentications publickey
IdentityFile ~/.ssh/id_rsa
User guccang

# github01
Host github01.com
HostName github01.com
PreferredAuthentications publickey
IdentityFile ~/.ssh/id_rsa
User guccang
```
