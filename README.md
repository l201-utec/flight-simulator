# Flight Dogfight Simulator (Unity 3D)

A flight simulator for 2020-1's Open Day.

## Dependencies

- Unity 2019.2.8f1

## Work guidelines (IMPORTANT)

For collaboration to work, the following plugins should be installed and used.

- Get Github For Unity plugin at [this link](https://unity.github.com/) or install it on the Unity Asset Store in your Editor.
    - It is recommended to work with the Command Line it provides, since it will save tons of weird crashes. You can open one terminal instance from `Window -> Github Command Line`.
    - To start, checkout to the `dev` branch.
- Get Unity Standard Assets (2017) from the Asset Store in your Editor. Don't worry, it'll be alright :^)


*Guideline #1*: Do **not** commit to the `master` branch.

Instead, create a branch from the `dev` branch addressing an issue. Use this format `fix/{number}`. 

Example: Let's say you're working on issue `#2`.
1. `git checkout dev`
2. `git checkout -b fix/2`

*Guideline #2*: If you want to develop something (or notice that something has to be done), be it adding some feature or fixing a bug, *create an issue*, please.

Steps:

1. Go to the [issue page](https://github.com/l201-utec/flight-simulator/issues).
2. Click on *New issue*.
3. Set a descriptive title, i.e. `Implement Cloud Physics`.
4. Set a description of what has to be achieved. The more detail it has the better will be for everyone.
5. Add a label: `bug` or `enhancement` (if it's a feature).
6. Click on *Projects* and select *Open Day Taskboard*.
7. (Optional) If *you* want to work on this issue, assign yourself on *Assignees*.
8. Submit new issue.

*Guideline #3*: If you finish an issue, **do not merge it without authorization**. Instead, push your branch to remote and make a pull request.

Example: Let's say you were working on issue `#2` and you finished your changes.

1. `git add .` (or the folders/files you want to commit.
2. `git commit -m "Fixes #2"`
3. `git push origin fix/2`
4. Go to the [pull request page](https://github.com/l201-utec/flight-simulator/pulls).
5. Create a `New pull request`.
6. Select `dev` as base branch and your branch (`fix/2`, in this case) as the compared branch.
7. If there are merge conflicts, fix them or message one of the collaborators in this project.
8. Click on `Create pull request`.
9. Wait upon approval.


## Collaborators 

- Diego Canez ([@dgcnz](https://github.com/dgcnz))
- Facundo Garcia ([@fgarciacardenas](https://github.com/fgarciacardenas))
- Nelson Soberon ([@Nelsonxxji](https://github.com/Nelsonxxji))


*Names are alphabetically ordered, they don't reflect the quantity of work each member has done.*
