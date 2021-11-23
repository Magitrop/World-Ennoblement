# World-Ennoblement

Данный репозиторий содержит исходный код и исполняемый файл компьютерной 3D игры-головоломки **"World Ennoblement"**.

Чтобы запустить игру, скачайте папку **Game** и, не изменяя и не перемещая ее содержимое, запустите файл **WorldEnnoblement.exe**.

**Язык программирования:** `C#`

**Игровой движок:** `Unity 2018`

## Правила игры
Сам уровень состоит из поля **N** x **M** клеток, где **N** и **M** могут варьироваться от 3 до 4. Каждая клетка содержит в себе плитку определенного типа
(например: **лес**, **горы**). Также всегда наличествует единственная клетка, в которой отсутствует плитка, за счет которой осуществляется
перемещение всех плиток по полю. Некоторые плитки имеют собственные условия (например: должна стоять рядом с плиткой с типом **"Горы"**,
не должна стоять рядом с плиткой с типом **"Лес"**), каждое из которых обязательно к выполнению. Лишь при выполнении каждого условия каждой плитки, 
содержащей условия, уровень будет считаться завершенным, а игрок получит за него звезду.

Перемещая плитки с помощью мыши, игрок должен расставить их в соответствии с некоторыми условиями, разнящимися от уровня к уровню.
По нажатию левой кнопкой мыши по плитке игроку будет показан список условий этой плитки, а сама плитка будет подсвечена; 
при повторном нажатии по ней она будет передвинута на свободную клетку, если рядом с ней такая есть.

Если на момент завершения уровня игрок сделал не более некоторого количества **X** перемещений плиток, то он получит дополнительную звезду
за прохождение этого уровня. Если же игрок сделал не более **X** и не более **Y** перемещений плиток (при этом **Y** < **X**), то он получит
сразу две дополнительные звезды за прохождение этого уровня.

Поскольку количество уровней не ограничено разумными пределами, конечной целью игрока является пройти как можно большее их количество, получив за
каждый пройденный уровень максимальное количество звезд.

## Игровой процесс
В окошке **Conditions** указаны условия конкретной клетки.<br/>
Здесь: клетка не должна стоять рядом с плиткой с типом **Лес**, **Горы** или **Озеро**, 
но должна стоять рядом с хотя бы одной плиткой с типом **Деревня**.
![image](https://user-images.githubusercontent.com/51723813/143051350-ad22403e-e7ad-495b-b978-2525df2c66b4.png)

В строке **Moves count** записаны соответственно 
1) число перемещений, которые игрок сделал на текущий момент
2) число перемещений для получения одной дополнительной звезды
3) число перемещений для получения двух дополнительных звезд
![image](https://user-images.githubusercontent.com/51723813/143052841-b6c579a4-989a-4693-8d77-3da9becc0063.png)

Когда все условия плиток выполнены, игрок должен нажать кнопку **End** в левом нижнем углу экрана.<br/>
Здесь, например, число перемещений, которые сделал игрок, превысило максимальное число перемещений для получения хотя 
бы одной дополнительной звезды, поэтому игрок получил лишь одну звезду.<br/>
Чтобы переключиться на следующий или предыдущий уровень, можно воспользоваться стрелками на клавиатуре.
![image](https://user-images.githubusercontent.com/51723813/143053705-de45e2fc-5f0e-433e-ae48-62ffaca5268f.png)

**Чтобы покинуть игру, вам придется использовать сочетание клавиш Alt + F4!**

## Примечания
**Не является** законченным программным продуктом.

**Присутствуют** недоработанные и тестовые модули. 

Корректность поведения программы и отсутствие ошибок времени выполнения **не гарантируются**.
