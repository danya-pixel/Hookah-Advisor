# Hookah-Advisor
Чат-бот в Telegram, помогающий выбрать табак для кальяна.

Участники
---------
- Травников Владислав - 19144
- Сахаров Данил - 19144
- Гарипов Тимур - 19144

Проблема
------

Иногда, даже опытным любителям кальяна трудно определиться с выбором табака, не говоря уже о начинающих. Люди тратят силы и время на поиск подходящего по настроению, вкусу и предпочтениям табака.  

Потенциальные пользователи
-------------------------------
- Пользователь, не имеющий опыта в выборе табака, не знает, какие предложения существуют и какие из них наиболее подходящие и качественные. 
- Пользователь, уже имеющий имеющий опыт в курении кальянов, хочет выбрать новый вкус, но не осведомлен о новинках и их качестве.
- Пользователь хочет следить за своими предпочтениями, за тем, что он уже пробовал, понравилось ему или нет, какие характеристи он отметил(крепость, вкус и тд)). 

MVP
--------------------------------
1. Проверка 18+ и имя пользователя для обращения к нему
2. Описание функциональности (/help)
3. Опросник 
4. Самый простой классификатор (по ответам на опросник)
5. Создать функции "поиск" и "рекомендации"

Основные компоненты системы
---------------------------

- Представление
  - Bot API
  - Модуль взаимодействия с Bot API (Общается с Bot API запросами)
  - Модуль парсинга ответа Телеграма (Преобразует JSON в объект Сообщение, вызывает соответствующие ) 
  - Модуль формирования ответа пользователю 
- Модель
- База данных
- Модуль взаимодействия с базой данных


Точки расширения
----------------
- Добавление карты ближайших табачных магазинов 
- Добавление возможности ведения личной истории табаков пользователя
- Добавление составлять рекомендации на основе истории пошьзователя (его предпочтений)
- Увеличение точности рекомендательной функции за счёт расширения базы данных, полученной за счёт сбора информации от пользователей.

Основные сценарии
----------------
- Сценарий 0 
  - Пользователь начинает работать с ботом.
  - Заходит в telegram, вводит «/start» и получает информацию о возможностях и то, как им пользоваться.
- Сценарий 1
  - Пользователь хочет получить рекомендацию табака
  - Пользователь выбирает функцию «Рекомендация»
  - Бот выводит ряд вопросов для конкретизации желаний пользователя (вкус, крепость, необычность)
  - Бот выводит рекомендации основываясь на запросах пользователя, рейтингах, отзывах.
- Сценарий 2
  - Пользователь хочет вести историю того, что он пробовал, а также иметь возможность оценивать то, что пробовал.
  - Пользователь выбирает функцию «История» 
  - Сценарий 2.1
    - Пользователь хочет посмотреть свою историю
    - Пользователь выбирает функцию «Посмотреть»
    - Бот выводит список табаков
   - Сценарий 2.2
     - Пользователь хочет пополнить историю
     - Пользователь выбирает функцию «Пополнить»
     - Бот предлагает ввести название табака
     - Бот задает ряд вопросов по оценке табака (оценка вкуса, крепость и тд)  
     - По окончании опроса отзыв пользователя сохраняется в базу данных 
- Сценарий 3
  - Пользователь хочет получить информацию о ближайших табачных магазинах по близости
  - Пользователь выбирает функцию «Купить» 
  - Бот кидает разрешение на получение геопозиции пользователя
  - Бот предлагает ближайшие табачные магазины


