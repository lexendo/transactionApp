# NotSoEpicApp

**NotSoEpicApp** is a desktop application built with C# and .NET using WPF (Windows Presentation Foundation) desktop application for managing personal finances with support for user roles and supervision features.

## 🧠 Motivation

This application was originally created years ago as a personal learning project. The primary goal was to understand how to work with databases — especially implementing **CRUD operations** — and to get familiar with the basics of **C#** and desktop development using **WPF**.

---

## Tech Stack

- **WPF (.NET)**
- **Database**: PosgreSQL
- **Languages**: C#, XAML
- **Data Access**: Raw SQL with helper methods for CRUD operations

---
## Features

- **User Authentication**
  - Register a new account
  - Login securely (with password hashing)
  - Supports normal users and supervisor mode

- **Language Support**
  - Seamless switching between English and Slovak (🇬🇧 🇸🇰)

- **Transaction Management**
  - Add new transactions with:
    - Title, amount, date, type, description, and income/expense toggle
  - Transaction types include: Entertainment, Groceries, Gift, Rent, Tax, etc.
  - Transactions are displayed in a sortable, scrollable, ordered table
  - Transactions affect current account balance in real-time

  ![obrázok](https://github.com/user-attachments/assets/93a205eb-4fba-4f13-accd-058143fa248a)

- **Graphical Overview**
  - View several financial charts
    - income vs expenses
    - incomes or expanses by category
    - balance over time
    - income vs expenses over time

  ![obrázok](https://github.com/user-attachments/assets/d65d6756-f24d-4552-8d05-733e9d771d38)
  
- **Supervisor System**
  - Users can assign **other users as supervisors** of their account
  - Supervisors may act on behalf of the user, depending on the permissions granted
  - A supervisor can access the user's account and perform actions the user explicitly allowed (view transactions, charts, manage others, etc.)

- **Permissions System**
  - Fine-grained control over what a supervisor can do in the account
  - Permissions include: view/add transactions, view charts, view/manage supervised users, assign new supervisors, etc.
 
  ![obrázok](https://github.com/user-attachments/assets/0c1fcba2-e801-4917-9996-55a348893628)



## 🎥 Demo Video

A short video demonstration of the app in action is available here:  
PLACEHOLDER



------
---

## 🇸🇰 Slovenská verzia


**NotSoEpicApp** je desktopová aplikácia vytvorená v jazyku **C#** a frameworku **.NET** pomocou technológie **WPF (Windows Presentation Foundation)**. Slúži na správu osobných financií a obsahuje podporu používateľských rolí a funkcie dohľadu (supervízie).

## 🧠 Motivácia

Táto aplikácia vznikla pred rokmi ako osobný projekt s cieľom naučiť sa pracovať s databázami, najmä vykonávať **CRUD operácie** a získať základnú skúsenosť s jazykom **C#** a desktopovým vývojom cez **WPF**.

---

## Použité technológie

- **WPF (.NET)**
- **Databáza**: PostgreSQL
- **Jazyky**: C#, XAML
- **Práca s databázou**: Priame SQL dotazy s pomocnými metódami

---

## Funkcionalita

- **Prihlásenie a registrácia**
  - Možnosť vytvoriť si nový účet
  - Prihlásenie s hashovaním hesla
  - Podpora bežného používateľa a režimu supervízora

- **Podpora jazykov**
  - Plynulé prepínanie medzi angličtinou a slovenčinou (🇬🇧 🇸🇰)

- **Správa transakcií**
  - Pridanie novej transakcie s:
    - názvom, sumou, dátumom, typom, popisom a prepínačom príjem/výdavok
  - Typy transakcií: zábava, potraviny, dar, nájom, dane a iné
  - Zoznam transakcií je triediteľný a prehľadný
  - Transakcie okamžite ovplyvňujú aktuálny zostatok

- **Grafické prehľady**
  - Zobrazenie grafov:
    - príjmy vs výdavky
    - rozdelenie podľa kategórií
    - vývoj zostatku v čase
    - vývoj príjmov a výdavkov v čase

- **Systém supervízorov**
  - Používateľ môže priradiť **iných používateľov ako svojich supervízorov**
  - Supervízori môžu konať v mene používateľa, podľa udelených oprávnení
  - Môžu napríklad vidieť transakcie, grafy, spravovať iných používateľov atď.

- **Oprávnenia**
  - Detailná kontrola oprávnení pre každého supervízora
  - Možnosti zahŕňajú: zobrazenie/pridanie transakcií, zobrazenie grafov, správa dohliadaných účtov, prideľovanie ďalších supervízorov atď.

---

## 🎥 Ukážkové video

Krátke video ukazujúce funkcie aplikácie si môžete pozrieť tu:  
ODKAZ NA VIDEO BUDE DOPLNENÝ



