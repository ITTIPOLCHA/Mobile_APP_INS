window.requestNotificationPermission = async (publicKey) => {
    const permission = await Notification.requestPermission();
    if (permission === 'granted') {
        // ลงทะเบียนเพื่อรับ Token (สำหรับส่งหาพนักงานคนนี้)
        const registration = await navigator.serviceWorker.ready;
        const subscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: publicKey // ต้องเอามาจาก Firebase
        });

        console.log(JSON.stringify(subscription));
        return JSON.stringify(subscription); // ส่งค่านี้กลับไปเก็บที่ฐานข้อมูล Server
    }
    return null;
};

window.getSubscriptionState = async () => {
    // 1. เช็กว่า Browser รองรับหรือไม่
    if (!('serviceWorker' in navigator) || !('PushManager' in window)) {
        return "NotSupported";
    }

    // 2. เช็กสิทธิ์ (Permission)
    if (Notification.permission === 'denied') {
        return "Denied";
    }

    // 3. เช็กว่ามีการลงทะเบียนกับ Push Server หรือยัง
    const registration = await navigator.serviceWorker.ready;
    const subscription = await registration.pushManager.getSubscription();

    if (subscription) {
        return "Subscribed";
    } else {
        return "NotSubscribed";
    }
};

window.unSubscribeNotifications = async () => {
    const registration = await navigator.serviceWorker.ready;
    const subscription = await registration.pushManager.getSubscription();

    if (subscription) {
        // ยกเลิก Subscription ในระดับ Browser
        const result = await subscription.unsubscribe();
        if (result) {
            console.log("ยกเลิกการลงทะเบียนสำเร็จ");
            // ส่งค่า JSON ของ subscription เดิมกลับไปเพื่อให้ Server ลบออกจากฐานข้อมูล
            return JSON.stringify(subscription);
        }
    }
    return null;
};