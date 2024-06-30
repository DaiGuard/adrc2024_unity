# adrc2024_unity

自動運転ミニカーバトル2024のシミュレーション用Unityプロジェクト

<img src="https://github.com/DaiGuard/adrc2024_unity/assets/26181834/639e1a6b-eefe-48b2-b59a-4d0ea1129ba2" width="60%">

--- 

## 自動運転ミニカーバトルとは

ラジコンを走らせる周回コースを自動運転で3周走りきるタイムを競う競技
開催ごとに周回後に駐車したり、コース上の障害物を倒すことで加点される

<img src="https://github.com/DaiGuard/adrc2024_unity/assets/26181834/972905d8-03f2-42a8-bb1b-6faec7306470" width="60%" />

## マシン説明

### 機器リスト

* PC ([Jetson Orin Nano](https://www.macnica.co.jp/business/semiconductor/manufacturers/nvidia/products/141900/))
* フロントカメラ ([B0287](https://akizukidenshi.com/catalog/g/g116713/))
* リアカメラ ([B0287](https://akizukidenshi.com/catalog/g/g116713/))
* 2Dライダ ([YDLIDAR X4](https://jp.robotshop.com/products/ydlidar-x4-360-laser-scanner?gad_source=1&gclid=CjwKCAjw1K-zBhBIEiwAWeCOFzqKt78xO4GaJjrxXtmsVIb9f7lvgIPl9DXz2WijCYVm1bOHOWXIYhoCEqYQAvD_BwE))
* ToFセンサ ([AE-VL53L1X](https://akizukidenshi.com/catalog/g/g114249/))
* IMU ([BMX055](https://akizukidenshi.com/catalog/g/g113010/))

### マシン3Dモデル
3Dモデル ([Googleドライブ](https://drive.google.com/drive/folders/1hydsSAonMya7ms5FJsZna2d9e6WoHrgz?usp=sharing))

### 外観

<img src="https://github.com/DaiGuard/adrc2024_unity/assets/26181834/fa439394-52d1-484b-9d49-80b9b5965518" width="60%" />

## シミュレータ構成

<img src="https://github.com/DaiGuard/adrc2024_unity/assets/26181834/ea5d4f70-c52c-4862-a1a8-ad960ce3bbb5" width="60%" />

* /cmd_vel (geometry_msgs/Twist)

    ロボット動作指示

* /imu (sensor_msgs/Imu)

    IMUセンサ値

* /range_[right, center, left] (sensor_msgs/Range)

    ToFセンサ値

* /scan (sensor_msgs/LaserScan)

    2D Lidarセンサ値

* /front_camera/compressed (sensor_msgs/CompressedImage)

    前面カメラ値

* /rear_camera/compressed (sensor_msgs/CompressedImage)

    後面カメラ値

---

## シミュレータ起動方法

### ROS-TCP-Endpointの起動

Unity側の`ROS-TCP-Connector`と接続するための`ROS-TCP-Endpoint`を起動する

[ROS-TCP-Endpoint (GitHub)](https://github.com/Unity-Technologies/ROS-TCP-Endpoint)

```bash
# ワークスペースの作成
mkdir -p colcon_ws/src
cd colcon_ws
# リポジトリをクローンする
git clone https://github.com/Unity-Technologies/ROS-TCP-Endpoint src/ROS-TCP-Endpoint
# ビルドする
colcon build --symlink-install
# 起動する
source install/local_setup.bash
ros2 run ros_tcp_endpoint default_server_endpoint
```

### 圧縮画像の解凍

Unity側から送信されるカメラ画像データは圧縮形式のため解凍して表示できるようにする

```bash
# Terminal 1
ros2 run image_transport republish compressed raw \
--ros-args -r /in/compressed:=/front_camera/compressed -r /out:=/front_camera/raw
# Terminal 2
ros2 run image_transport republish compressed raw \
--ros-args -r /in/compressed:=/rear_camera/compressed -r /out:=/rear_camera/raw
```

### レビュー

<img src="https://github.com/DaiGuard/adrc2024_unity/assets/26181834/d97a9c24-836e-4bd9-aedf-8d638435aff1" width="60%" />

## トラブルシュート

### 'HeaderMsg' does not contain a definition for 'seq' 

```
Library\PackageCache\com.frj.unity-sensors-ros@c33247ddf2\Runtime\Scripts\Serializers\TF\TFMsgSerializer.cs(37,50): error CS1061: 'HeaderMsg' does not contain a definition for 'seq' and no accessible extension method 'seq' accepting a first argument of type 'HeaderMsg' could be found (are you missing a using directive or an assembly reference?)
```

<img src="https://github.com/DaiGuard/adrc2024_unity/assets/26181834/623194a4-dbfe-425b-88f0-a68b39585d30" width="60%"/>

UnityプロジェクトをROS2に設定した際に、下記のようなエラーが発生した場合は`Library\PackageCache\com.frj.unity-sensors-ros@c33247ddf2\Runtime\Scripts\Serializers\TF\TFMsgSerializer.cs`の37行目をコメントアウトする

変更前

```c#
    TransformStampedMsg transform = new TransformStampedMsg();
    transform.header.seq = headerMsg.seq;
    transform.header.stamp = headerMsg.stamp;
```

変更後
```c#
    TransformStampedMsg transform = new TransformStampedMsg();
    // transform.header.seq = headerMsg.seq;
    transform.header.stamp = headerMsg.stamp;
```

