#include <iostream>
#include <vector>
#include <string>

using namespace std;

struct fraction {
    int num; // числитель
    int den; // знаменатель
};
fraction read() {
    fraction f;
    while (true) {
        cout << "Введите числитель: ";
        cin >> f.num;
        if (cin.fail()) {
            cin.clear(); // очищаем флаги ошибок
            cin.ignore(numeric_limits<streamsize>::max(), '\n'); // игнорируем оставшиеся символы до конца строки
            cout << "Ошибка ввода. Пожалуйста, введите целое число." << endl;
        }
        else {
            break;
        }
    }
    while (true) {
        cout << "Введите знаменатель: ";
        cin >> f.den;
        if (cin.fail() || f.den == 0) {
            cin.clear(); // очищаем флаги ошибок
            cin.ignore(numeric_limits<streamsize>::max(), '\n'); // игнорируем оставшиеся символы до конца строки
            cout << "Ошибка ввода. Пожалуйста, введите целое число, отличное от нуля." << endl;
        }
        else {
            break;
        }
    }
    return f;
}
void print(fraction f) {
    cout << f.num << "/" << f.den << endl;
}
int nod(int a, int b) {
    while (b != 0) {
        int temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}
fraction Short(fraction f) {
    int NOD = nod(f.num, f.den);
    f.num /= NOD;
    f.den /= NOD;
    return f;
}
fraction add(fraction a, fraction b) {
    fraction result;
    result.num = a.num * b.den + b.num * a.den;
    result.den = a.den * b.den;
    return Short(result);
}

struct Student {
    string fio; // ФИО
    int age; // возраст
    int course; // курс
    int grades[3]; // успеваемость (оценки по 3 предметам)
};
void inputStudent(Student& student) {
    cout << "Введите ФИО: ";
    getline(cin >> ws, student.fio);
    cout << "Введите возраст: ";
    cin >> student.age;
    cout << "Введите курс: ";
    cin >> student.course;
    cout << "Введите оценки по 3 предметам: ";
    for (int i = 0; i < 3; i++) {
        cin >> student.grades[i];
    }
}
void printStudent(const Student& student) {
    cout << "ФИО: " << student.fio << endl;
    cout << "Возраст: " << student.age << endl;
    cout << "Курс: " << student.course << endl;
    cout << "Оценки по 3 предметам: ";
    for (int i = 0; i < 3; i++) {
        cout << student.grades[i] << " ";
    }
    cout << endl;
}
int countFailingStudents(const vector<Student>& students, int course) {
    int count = 0;
    for (const auto& student : students) {
        if (student.course == course) {
            bool isFailing = false;
            for (int i = 0; i < 3; i++) {
                if (student.grades[i] < 4) {
                    isFailing = true;
                    break;
                }
            }
            if (isFailing) {
                count++;
            }
        }
    }
    return count;
}

int main() {
    setlocale(LC_ALL, "Russian");
    cout << "Номера задач: \n";
    cout << "Выход из программы - 0 \n";
    cout << "Задача 1 - 1\n";
    cout << "Задача 2 - 2\n";

    while (true) {
        short int n;
        cout << "Введите номер задачи: ";
        cin >> n;
        cin.ignore();

        switch (n) {
        case 0: {
            cout << "Завершение программы." << endl;
            return 0;
        }
        case 1: {
            fraction f1 = read();
            fraction f2 = read();
            fraction sum = add(f1, f2);
            cout << "Сумма дробей: ";
            print(sum);
            break;
        }
        case 2: {
            int n;
            cout << "Введите количество студентов: ";
            cin >> n;
            vector<Student> students(n);
            for (int i = 0; i < n; i++) {
                cout << "Введите информацию о студенте " << i + 1 << ":" << endl;
                inputStudent(students[i]);
            }

            cout << "Введите номер курса для проверки: ";
            int course;
            cin >> course;

            int failingStudents = countFailingStudents(students, course);
            cout << "Количество неуспевающих студентов на курсе " << course << " : " << failingStudents << endl;
            break;
        }
        default: {
            cout << "Неверный номер задачи. Попробуйте снова." << endl;
            break;
        }
        }
    }

    return 0;
}
