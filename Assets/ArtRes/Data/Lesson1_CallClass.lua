--lua使用C#的类很简单
--固定写法
--CS.命名空间.类名
--Unity的类 ——CS.UnityEngine.类名
--CS.UnityEngine.GameObject

--实例化一个对象 类名后面写括号 调用无参构造
local obj = CS.UnityEngine.GameObject()
local obj2 = CS.UnityEngine.GameObject("唐老狮")
--为了方便使用 和节约性能 定义全局变量存储常用类
GameObject = CS.UnityEngine.GameObject
local obj3 = GameObject("唐老狮好爱你")

--使用类中的静态方法 用.调用即可
local obj4 = GameObject.Find("唐老狮")

Debug = CS.UnityEngine.Debug
Vector3 = CS.UnityEngine.Vector3

--使用对象中的成员变量 用点访问
Debug.Log(obj4.transform.position)

--使用成员方法 一定要加:
obj4.transform:Translate(Vector3.right)
Debug.Log(obj4.transform.position)

--自定义的类 不继承mono
local t = CS.MrTang.Test()
print(t.i)
print(t.str)
t:Speak("哈哈哈")
CS.MrTang.Test.Eat()

--继承mono的类 一定是通过AddCompment使用
local obj5 = GameObject("加脚本测试")
--Lua中不支持泛型 但xlua提供 typeof方法 得到类型
obj5:AddComponent(typeof(CS.LuaCallCharp))

--总结
--使用C#中的类
--CS.命名空间.类名
--CS.UnityEngine.类名
--没有命名空间的  CS.类名

--实例化一个对象
--CS.命名空间.类名()

--静态方法和变量
--CS.命名空间.类名.方法或变量

--成员变量
--实例化的对象.变量名
--成员方昂发
--实例化的对象:方法名

