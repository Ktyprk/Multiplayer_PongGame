Pong Game With Pun 2

Karşılaştığım zorluklar:  
1. Gün
- Joyistick kontroller çalışmamam sorunu yaşadım.    
- Oda kurma, Odaya katılma ve nickname ayarlama yapıldı.
- Score boardı nasıl iki taraftada günceleyecğimi öğrendim.

2. Gün
- 2 Taraftada UI Güncellemesini çözdüm.
- 2 tarafında topla etkileşime geçmesini ve topun fiziksel çalışmasında sorun yaşadım.
- Top çok laglı çalışıyordu.

3. Gün
- Serverdaki lagı nasıl hesaplayacağımı ve 2 oyuncayada aynı gecikmeyi nasıl vereceğimi öğrendim ve uyguladım.
- 2 Oyuncununda Odaya bağlanmadan oyun başlamamasını sağladım.
- Oyuna oyun süresi ekledim.

4. Gün
- Joyistick kontrol sorununu çözdüm.
- Oyun bittiğinde kazanan ve kaybeden tarafın belirlenmesini sağladım.
- Win ve lose ekranlarının çıkmasını sağladım.
- UI Düzenlemeri yapıldı.


4 oyunculu yapma ihtimalime karşılık Gol atan kişinin puanını artmasını sağladım. Fakat kendi kalesine gol attığındada kendi puanı arttığı için ve oyun 2 kişilik olduğu için şu anda kişi bazlı puan artmaktadır. Kodu Aşağıda  
 private void OnTriggerEnter2D(Collider2D other) {              
               if (pw.IsMine && other.CompareTag("Goal")) {                 
                     if (otherPV != null) {       
                     Player scoringPlayer = otherPV.Owner;       
                        if (scoringPlayer != null) {            
                          lastScoringPlayerNickname = scoringPlayer.NickName;
                            GameManager.instance.ShowScore(scoringPlayer, 1);
                              }  }         
                   pw.RPC("ResetBall", RpcTarget.All);         
                } }       
