## HTTP2 SERVER

gen cert.

```
dotnet dev-certs https -ep ./k8s/configmap/.aspnet/https/aspnetapp.pfx -p password
```

build

```shell
docker build -t guitarrapc/http2-dotnet -f Http2Server/Dockerfile .
docker tag guitarrapc/http2-dotnet guitarrapc/http2-dotnet:0.1.0
docker push guitarrapc/http2-dotnet:0.1.0
```

deploy

```shell
k kustomize ./k8s/common/development | kubectl apply -f -
```

## Request diagnostics

HTTP/2 upgrade

```
Host: localhost:5001
Scheme: https
Method: GET
Path: /weatherforecast
Protocol: HTTP/2
Headers:
  Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
  Accept-Encoding:gzip, deflate, br
  Accept-Language:en-GB,en-US;q=0.9,en;q=0.8
  Cookie:JSESSIONID.3543ec78=node01iqci99oh7hyi1x2s3icogjp122.node0,JSESSIONID.f1b5ffda=node01dnxibg8yr3a1d0cgg7xcmt711.node0,jenkins-timestamper-offset=-32400000,JSESSIONID.d000738d=node01cwab9lmsljcidk0u5h7uhkzd1.node0,JSESSIONID.8315c409=node01jpfa6zhl7z631mflsnoni6mhi0.node0,JSESSIONID.2328a8aa=node0daul36aarf2akpz5k5xw7dno0.node0,JSESSIONID.f8deab25=node0tg9q1eryn4teezf450yrssrp1.node0,JSESSIONID.67085c6a=node0hf7latdc3dfe196mdiriplowg1.node0,JSESSIONID.557875b6=node01bf5bpjg5ft9bkgct4mthsw550.node0,JSESSIONID.2a2dabc1=node0fdffl4feztcc1uu15dag0bofq1.node0,JSESSIONID.8d6ef113=node01h0v4sun1g7en1w8em5xjlradq2.node0,JSESSIONID.6ed70633=node0f9r27f7ffuii13vk2rn3uu5m61.node0,JSESSIONID.b2fd5ac2=node01xev5qe564bo61h1bzx2fam9oa4.node0,JSESSIONID.628b43ff=node01mydzgva6glalwnsdx66ujff42.node0,JSESSIONID.bdc269dd=node0otg6mywburjca0ulq8i659811.node0,JSESSIONID.d29b2dc9=node01e1lxzfuls5jzwubcsx8n2out2.node0,jenkins-timestamper=system,jenkins-timestamper-local=true,JSESSIONID.21f3dbf4=node012brw84ct3m7ois2to6zhdl5p0.node0,JSESSIONID.e9050f33=node017s8kh9r54s48u54mxglcdsa72.node0,JSESSIONID.4e11d4c1=node0qejy1keezktz1spmt5e4i7m430.node0,JSESSIONID.243cab23=node012vojrz95zwa71qzlfzmron4ky0.node0,JSESSIONID.5d201c20=node01qq6wsqz4drwulmp9s2zxx6j0.node0,JSESSIONID.987bd4f3=node0j5rxmm9ok6wa1rkq6mtoyg2v70.node0,JSESSIONID.69a200ec=node01b1gwlrrxwtal3a5j79s0xeq70.node0,JSESSIONID.a2c88737=node0133t9cud9nlbq5n5mb9oj2jqp0.node0,JSESSIONID.f3a1be8a=node0yd80280y8i3p1tcs2nsell1q21.node0,JSESSIONID.8ae8e182=node01piwsu9pgf8w811wn054z6ahcq0.node0,JSESSIONID.3df459f2=node01ggaqcbp9sw8a12gwtc5jb3u5j3.node0,JSESSIONID.28a5c5b5=node0l2olwftooofouoinc7pdpvo41.node0,JSESSIONID.330a05dc=node0lyyul6kphidp1e7gem8z3xvgs1.node0,JSESSIONID.0a59918b=node0c1pmek5cpks1no11q8nv4cx10.node0,JSESSIONID.7abae32e=node0de5hh28oe5w74xnjpdmnxfz0.node0,JSESSIONID.90e810d2=node0ymqzwbouxa53cw0kk7mcl4gd0.node0,JSESSIONID.da2eb44a=node01os22s7mpql7oz09rnija9i040.node0,JSESSIONID.1129c458=node01bv4jjexohpmda4pq87z01hq2.node0,JSESSIONID.765cd996=node0iygab4wrlpbb1asr4ufs14l931.node0,JSESSIONID.74304318=node01t35rumhikmtyjp7ii2zzcis31.node0,JSESSIONID.1aef90b3=node01fygg5vjwu6ii6yw277oodavi2.node0,JSESSIONID.8c88ac11=node01cql0trkvgwjxwk5s9ig2meq28.node0,JSESSIONID.0bea1512=node0xr840jqbsl991xva5xr7xsk5p0.node0,JSESSIONID.bad84f1a=node0197fxsz9un2q246ymprh7irt60.node0,JSESSIONID.88d33e29=node0sjk736o6cu8j1eikrza6g7qgq0.node0,JSESSIONID.8c2189e2=node01xyg4umlafyvr1vr20r394v9mb1.node0,JSESSIONID.24724d5b=node0u69ekl9jgw6r11td4jjsweec0.node0,JSESSIONID.29d108eb=node0o75k4lh0hizx11laxpalnzndx0.node0,JSESSIONID.a4d66c92=node012zxw4nk8kqcronrxib3w1ls2.node0,JSESSIONID.30a91b98=node016sfhis4vlcmv1ucx7vdpcfbyt0.node0,JSESSIONID.b2a3adac=node06f6o9gt65bt7ylyqjlusi6so0.node0,JSESSIONID.42608dea=node01r4h0xhb6t9eqldih7gxwqczp0.node0,JSESSIONID.bc19a184=node0w20i63lwnodsv4oziusslhi0.node0,JSESSIONID.b3e38093=node01uqukdk9dv5571f3sdmfwqa40y0.node0,JSESSIONID.c9a33605=node0skb0p9icdylh1iazzsqkg847y1.node0,JSESSIONID.ab501621=node01puj1rgseosfl787n356tndl23.node0,.AspNetCore.Antiforgery.nZ3ePmm07do=CfDJ8IL9fMldpVBEmGee-vdRmn_IkwKfkq0Stk_r5mp_h4s_40dC-qVyR0O1KkzyhM4U-9IoCBmf1WYbWeywt98hLBE_L3a4mrxxDVc_7I_d3mQpl3bpcYbVuB7IAkoYbeuzEt7Vvi4JvFJzkVdZ4zkPRpA,JSESSIONID.d91e953f=node0t4kjdj4g48djy0xlktfnw8w51.node0,screenResolution=3840x2160,JSESSIONID.4d25f9d9=node0116wfy9yyvze52t9bsxczn2cw1.node0,JSESSIONID.e117d7bd=node0ecruttwv02si5uok02mzcs11.node0,JSESSIONID.361afd66=node015rl70fk99re28k6cow7o8fx20.node0,JSESSIONID.f5e8cbd9=node0vjgfjhd2vinved870vaelnek0.node0,JSESSIONID.4f99ea1c=node0t0t1lvynerwsgjcq7db9i0600.node0,.AspNetCore.Antiforgery.9TtSrW0hzOs=CfDJ8A4YAHrrMg9PmndT4JyAl1_BIZ9sJpm6vmBdta7me8MRay0cmedkNWEz3G_OM-1IibhVAHk6cjkz26N0lybrIMMA4ajBvLfU8w13zLW2HQuOUa5t3oqWfl_sb7hmI-x1S-KDCAUg40o8kA5snAXtdYc
  Host:localhost:5001
  User-Agent:Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36
  Upgrade-Insecure-Requests:1
  :method:GET
  :authority:localhost:5001
  :scheme:https
  :path:/weatherforecast
  sec-fetch-site:none
  sec-fetch-mode:navigate
  sec-fetch-user:?1
  sec-fetch-dest:document
  ```
