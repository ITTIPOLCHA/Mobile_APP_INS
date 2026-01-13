/* Manifest version: WtTrRruk */
// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('fetch', () => { });
// --- ��ǹ���������������Ѻ Notification ---
self.addEventListener('push', event => {
    
    try
    {
        const payload = event.data ? event.data.json() : { title: '����͹����', body: '�բ�ͤ�������ҡ�к�' };

        const title = payload.title;
        const options = {
            body: payload.body,
            icon: 'icon-512.png',
            badge: 'icon-192.png',
            vibrate: [200, 100, 200],
            data: { url: '/' } // ��� URL ����ͧ�������Դ����͡���� Notification
        };

        event.waitUntil(
            self.registration.showNotification(title, options)
        );
        console.log("Notification:", payload.body);
    }
    catch (error)
    {
        console.error("Notification Error:", error);
    }
});

self.addEventListener('notificationclick', event => {
    event.notification.close();
    event.waitUntil(
        clients.openWindow(event.notification.data.url)
    );
});
