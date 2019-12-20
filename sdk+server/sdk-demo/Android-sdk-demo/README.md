登陆保护 Android sdk 示例 demo
===

### demo 运行步骤

* 1、运行模拟业务后端：check demo，运行方法见login-protection-check-demo目录

* 2、修改 LoginActivity.java 的 onCreate，填入您的 productNumber，如下：
```
	watchman.init(mContext, "your productNumber",new RequestCallback(){
		@Override
		public void onResult(int code, String msg) {
			Log.e(TAG,"init OnResult , code = " + code + " msg = " + msg);
		}
	});
```
* 3、修改 LoginTask.java的 doInBackground 方法,在 params.put()中填入您的BusinessId,如下：
```
	String token = watchman.getToken( "your BusinessId",new RequestCallback(){
		@Override
		public void onResult(int code, String msg) {
			Log.e(TAG,"Register OnResult, code = " + code + " msg = " + msg);
		}
	});
	params.put("token", token);
```
* 4、修改 LoginTask.java的 PostData方法中的url变量
```
     String url = "http://localhost:8182/login.do";
     // 例如，替换如下：
     String url = "http://10.240.132.43:8182/login.do";
```
* 5、至此，配置和修改完成，编译运行即可
