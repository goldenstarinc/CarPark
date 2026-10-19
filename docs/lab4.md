# Лабораторная работа №4. Система контроля версий Git

В репозитории две истории разработки одной и той же программы.

| Ветка | Методология | Что видно в истории |
|---|---|---|
| `waterfall` | Каскадная | 5 коммитов, по одному на этап. Код появляется целиком только на 3-м этапе, тесты — на 4-м. |
| `main` | Agile (Scrum) | 3 спринта. В каждом спринте работает мини-программа и тесты. Есть ветка `feature/debts` и слияние. |

## Команды из задания
```
git init                               # создать репозиторий
git add .                              # подготовить файлы
git commit -m "сообщение"              # зафиксировать
git branch feature/debts               # создать ветку
git switch feature/debts               # перейти в ветку
git switch main                        # вернуться
git merge --no-ff feature/debts        # слияние веток
git log --graph --oneline --all        # дерево истории
git tag v1.0                           # метка версии
```

## Удалённый репозиторий
```
git remote add origin https://github.com/ВАШ_ЛОГИН/parking.git
git push -u origin --all
git push origin --tags
```
Второй участник команды: `git clone ...`, правки, `git commit`, `git pull`, `git push`.
Добавить его можно в настройках репозитория: Settings -> Collaborators.
