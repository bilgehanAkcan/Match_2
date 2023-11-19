Peak Mobile Case:
Oyunu başlatmadan önce GameCanvas'a tıklayıp Inspector'dan hamle sayısı, hedeflerden kaç tane başarılması gerektiği ve hedef nesnesi değiştirilebilir. Hedef nesnesini değiştirmek için 
Assets directory'sindeki "Peak Mobile Case V1" klasörünün içindeki "Images" klasöründen bir nesne Inspector'daki ilgili alana sürüklenip bırakılabilir.
Ayrıca, GameCanvas altındaki ItemGroup Game Object'ine tıklayıp Inspector'dan Grid'in row ve column sayısı değiştirilebilir.

!ÖNEMLİ!: Eğer GameCanvas ve ItemGroup Game Object'lerinin Inspector'da gerekli değerleri proje teslimi sırasında kaybolduysa yukarıda belirttiğim şekilde SerializeField'ların 
initialize edilmesi gerekmektedir. Bilgisayarımda çalışan projede değerler şu şekildedir:

ItemGroup:
Rows: 10
Columns: 10
Spacing: 5
Candy Prefabs: [cube_1, cube_2, cube_3, cube_4, cube_5, Balloon, Duck, rocket_left, rocket_right, cube_particle, rocket_up, rocket_bottom] => Length'i 12 olan CandyPrefabs array'inin
elemanları bu şekilde initialize edilmelidir. Bu elemanlara Assets directory'sindeki Prefabs klasöründen erişilebilir.

GameCanvas:
Moves Count: 45
Goal 2 Count: 30
Goal 1 Count: 25
Button 2 Sprite: "Green" adlı image
Button 1 Sprite: "Yellow" adlı image
Goal 2: Goal2 (Button) => BackgroundCanvas altındaki Goal2 butonu
Goal 1: Goal1 (Button) => BackgroundCanvas altındaki Goal1 butonu
Moves Text: MovesText => BackgroundCanvas altındaki MovesText field'ı

Maksimum deneyim için oyunu 16:9 Portrait modunda başlatmanız tavsiye edilir.