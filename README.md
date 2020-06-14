1. Читаем пресеты

2. Парсим выбранный пресет

	// На этом этапе считываем все модели в список и создаем прокси моделей, где указываем список разрешенных соседей для каждого направления (x-, x+, y-, y+, z-, z+)
	
	// Модель = 3D модель
	
3. Парсим все модели, что входят в пресет (читаем вершины и треугольники в память программы)

4. Создаем равномерную сетку (размеры читаем из пресета)

5. Запускаем WFC

6. Пока не обошли все клетки, выполняем:

	6.1. Наблюдение: находим элемент с наименьшей энтропией (клетка, которая имеет меньшее чисто возможных состояний).
	
			Для найденного элемента выбираем рандомно любое из возможных состояний.
			
			// Состояние = Id модели
			
	6.2. Распространение: каждому из 6 соседей найденного элемента (кроме уже рассмотренных) задаем все возможные состояния (исходя из того, что предписано в пресете)
	
			// Соседи: левый(x-), правый(x+), нижний(y-), верхний(y+), задний(z-), передний(z+)
			
7. На данном этапе мы должны иметь полностью развернутую сетку, то есть каждой ячейке сетки должено быть установлено значение - Id модели, которая будет помещена в ячейку

8. Для каждой ячейки, начиная с (0,0,0):

	8.0. Каждой вершины исходной модели: позиция вершины += (позиция клетки * размер вокселя)
	
	8.1. В выходную модель пишем аттрибуты вершин (позиция, нормаль и текс-коорд) и описываемые треугольники (id треугольника + общее кол-во вершин)
	
	8.2. Общее кол-во вершин += кол-во вершин модели в этой итерации
9. Сохранение модели в ФС

// Улучшение?: Не устанавливать финальное состояние сразу, а уменьшать число возможных состояний каждое наблюдение, пока не останется одно

// Баг: Убрать Wrap при нахождении соседей

// Проблема: Как действовать с пустыми ячейками? Заполнять здание "изнутри" моделями - невалидный кейс, поэтому используем пустые ячейки => "нутро" здания нельзя заполнять; нужно находить и пропускать такие ячейки

// Доработка: Симметрия и повороты моделей
