#include <iostream>
#include <fstream>
#include <vector>
#include <queue>
#include <stack>
#include <limits>
#include <string>
#include <sstream>
#include <iomanip>

using namespace std;

static int input_check(string message, int min, int max) {
    int input;
    while (true) {
        cout << message;
        cin >> input;
        if (cin.fail() || input < min || input > max) {
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
            cout << "Ошибка ввода! Введите число от " << min << " до " << max << "." << endl;
        }
        else {
            return input;
        }
    }
}
// ввод матрицы смежности
static void input_matrix_smegnosti(vector<vector<int>>& matrix, int& n) {
    n = input_check("Введите количество вершин графа (от 1 до 10): ", 1, 10);
    matrix.resize(n, vector<int>(n));
    cout << "\nВведите матрицу смежности размером " << n << "х" << n << ":\n";
    cout << "Ввод через пробел (только 0 или 1). Диагональ должна быть 0. Матрица должна быть симметричной.\n";

    for (int i = 0; i < n; i++) {
        while (true) {
            cout << "Строка " << i + 1 << ": ";
            bool valid = true;
            for (int j = 0; j < n; j++) {
                cin >> matrix[i][j];
                if (cin.fail() || (matrix[i][j] != 0 && matrix[i][j] != 1)) {
                    valid = false;
                    break;
                }
            }
            if (!valid || matrix[i][i] != 0) {
                cin.clear();
                cin.ignore(numeric_limits<streamsize>::max(), '\n');
                cout << "Ошибка! Повторите ввод строки.\n";
                continue;
            }
            break;
        }
    }

    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            if (matrix[i][j] != matrix[j][i]) {
                cout << "Матрица не симметрична! Повторите ввод.\n";
                input_matrix_smegnosti(matrix, n);
                return;
            }
}
static void save_matrix_smegnosti(const vector<vector<int>>& matrix) {
    ofstream fout("matrix_smegnosti.txt");
    for (const auto& row : matrix) {
        for (int val : row)
            fout << val << " ";
        fout << "\n";
    }
    fout.close();
}
// матрица инцидентности
static void build_matrix_incidence(const vector<vector<int>>& adj) {
    int n = adj.size();
    vector<pair<int, int>> edges;

    // Собираем список рёбер
    for (int i = 0; i < n; i++)
        for (int j = i + 1; j < n; j++)
            if (adj[i][j] == 1)
                edges.push_back({ i + 1, j + 1 });

    vector<vector<int>> inc(n, vector<int>(edges.size(), 0));

    // Заполняем матрицу инцидентности
    for (int e = 0; e < edges.size(); e++) {
        inc[edges[e].first - 1][e] = 1;
        inc[edges[e].second - 1][e] = 1;
    }

    ofstream fout("matrix_incidence.txt");

    fout << "   ";
    for (const auto& edge : edges)
        fout << setw(6) << "(" + to_string(edge.first) + "," + to_string(edge.second) + ")";
    fout << "\n";

    for (int v = 0; v < n; v++) {
        fout << v + 1 << " |";
        for (int e = 0; e < edges.size(); e++) {
            fout << setw(5) << inc[v][e];
        }
        fout << "\n";
    }

    fout.close();
}
// список инцидентных ребер
static void build_list_incidence_reber(const vector<vector<int>>& adj) {
    int n = adj.size();
    vector<pair<int, int>> edges;
    // список рёбер
    for (int i = 0; i < n; i++)
        for (int j = i + 1; j < n; j++)
            if (adj[i][j] == 1)
                edges.push_back({ i, j });

    ofstream fout("list_incidence_reber.txt");
    // для каждой вершины выводим список её рёбер
    for (int v = 0; v < n; v++) {
        fout << "Вершина " << v + 1 << ": ";
        for (int e = 0; e < edges.size(); e++)
            if (edges[e].first == v || edges[e].second == v)
                fout << "(" << edges[e].first + 1 << "-" << edges[e].second + 1 << ") ";
        fout << "\n";
    }
    fout.close();
}
// список смежных вершин
static void build_list_smegnih_vershin(const vector<vector<int>>& adj) {
    int n = adj.size();
    ofstream fout("list_smegnih_vershin.txt");
    for (int i = 0; i < n; i++) {
        fout << "Вершина " << i + 1 << ": ";
        for (int j = 0; j < n; j++)
            if (adj[i][j] == 1)
                fout << j + 1 << " ";
        fout << "\n";
    }
    fout.close();
}
static void DFS(const vector<vector<int>>& adj, int start) {
    int n = adj.size();
    vector<int> label(n, 0);       // метки вершин
    vector<bool> inStack(n, false);
    vector<bool> visited(n, false);
    vector<string> edgeTypes;
    stack<int> s;
    int k = 1;

    label[start] = k++;
    s.push(start);
    inStack[start] = true;

    cout << "\nDFS (обход в глубину):";

    while (!s.empty()) {
        int v = s.top();
        bool found = false;

        for (int w = 0; w < n; w++) {
            if (adj[v][w] == 1) {
                if (label[w] == 0) {
                    // Новая вершина — древесное ребро
                    label[w] = k++;
                    edgeTypes.push_back("(" + to_string(v + 1) + "-" + to_string(w + 1) + ") — древесное");
                    s.push(w);
                    inStack[w] = true;
                    found = true;
                    break;
                }
                else if (!visited[w]) {
                    // Уже помеченная вершина — обратное ребро
                    edgeTypes.push_back("(" + to_string(v + 1) + "-" + to_string(w + 1) + ") — обратное");
                }
            }
        }

        if (!found) {
            s.pop();
            inStack[v] = false;
            visited[v] = true;
        }
    }

    cout << "\nМетки вершин:\n";
    for (int i = 0; i < n; i++)
        cout << "Вершина " << i + 1 << ": метка " << label[i] << "\n";

    cout << "\nКлассификация рёбер:\n";
    for (const auto& edge : edgeTypes)
        cout << edge << "\n";
}
static void BFS(const vector<vector<int>>& adj, int start) {
    int n = adj.size();
    vector<int> key(n, -1);       // уровень вершины
    vector<int> parent(n, -1);    // родитель в дереве
    vector<bool> visited(n, false);
    queue<int> q;

    key[start] = 0;
    visited[start] = true;
    q.push(start);

    cout << "\nBFS (обход в ширину):\n";

    while (!q.empty()) {
        int v = q.front(); q.pop();
        cout << "Вершина " << v + 1 << " (уровень " << key[v] << ")\n";

        for (int w = 0; w < n; w++) {
            if (adj[v][w] == 1 && !visited[w]) {
                key[w] = key[v] + 1;
                parent[w] = v;
                visited[w] = true;
                q.push(w);
            }
        }
    }

    cout << "\nДерево поиска:\n";
    for (int i = 0; i < n; i++) {
        if (parent[i] != -1)
            cout << "Вершина " << i + 1 << " <- " << parent[i] + 1 << "\n";
    }
}
// загрузка матрицы
static bool load_matrix_smegnosti(vector<vector<int>>& matrix) {
    ifstream fin("matrix_smegnosti.txt");
    if (!fin.is_open()) {
        cout << "Файл matrix_smegnosti.txt не найден.\n";
        return false;
    }

    matrix.clear();
    string line;
    while (getline(fin, line)) {
        vector<int> row;
        int val;
        istringstream iss(line);
        while (iss >> val)
            row.push_back(val);
        matrix.push_back(row);
    }
    fin.close();
    return true;
}
int main() {
    setlocale(LC_ALL, "RU");

    while (true) {
        cout << "\nВыберите действие:\n";
        cout << "1. Ввод и преобразование графа\n";
        cout << "2. Обход графа из файла (BFS и DFS)\n";
        cout << "0. Выход\n";

        int choice = input_check("Ваш выбор: ", 0, 2);

        switch (choice) {
        case 0:
            cout << "Завершение программы.\n";
            return 0;

        case 1: {
            vector<vector<int>> matrix;
            int n;
            input_matrix_smegnosti(matrix, n);
            save_matrix_smegnosti(matrix);
            build_matrix_incidence(matrix);
            build_list_incidence_reber(matrix);
            build_list_smegnih_vershin(matrix);
            cout << "Граф сохранён и преобразования завершены.\n";
            break;
        }

        case 2: {
            vector<vector<int>> matrix;
            if (!load_matrix_smegnosti(matrix)) {
                cout << "Невозможно выполнить обход: файл matrix_smegnosti.txt не найден или повреждён.\n";
                break;
            }
            int n = matrix.size();
            int start = input_check("\nВведите номер стартовой вершины для обхода (от 1 до " + to_string(n) + "): ", 1, n);
            BFS(matrix, start - 1);
            DFS(matrix, start - 1);
            break;
        }

        default:
            cout << "Неизвестная команда. Повторите ввод.\n";
            break;
        }
    }
}

/* Пример матрицы 5х5
0 1 1 0 0
1 0 0 1 0
1 0 0 0 1
0 1 0 0 0
0 0 1 0 0
*/
/* Пример матрицы 7х7
0 1 1 0 0 0 0
1 0 0 1 1 0 0
1 0 0 0 0 1 0
0 1 0 0 0 0 0
0 1 0 0 0 0 1
0 0 1 0 0 0 0
0 0 0 0 1 0 0

*/
