#include <iostream>
#include <iomanip>
#include <queue>
#include <limits>
#include <string>
using namespace std;

struct Node {
    int key;
    Node* left;
    Node* right;
    int height;

    Node(int k) : key(k), left(nullptr), right(nullptr), height(1) {}
};

static int getHeight(Node* node) {
    return node ? node->height : 0;
}

static int getBalance(Node* node) {
    return node ? getHeight(node->left) - getHeight(node->right) : 0;
}

static void updateHeight(Node* node) {
    node->height = 1 + max(getHeight(node->left), getHeight(node->right));
}

static Node* rotateRight(Node* y) {
    Node* x = y->left;
    Node* T2 = x->right;

    x->right = y;
    y->left = T2;

    updateHeight(y);
    updateHeight(x);

    return x;
}

static Node* rotateLeft(Node* x) {
    Node* y = x->right;
    Node* T2 = y->left;

    y->left = x;
    x->right = T2;

    updateHeight(x);
    updateHeight(y);

    return y;
}

static Node* balance(Node* node) {
    updateHeight(node);
    int bf = getBalance(node);

    if (bf > 1) {
        if (getBalance(node->left) < 0)
            node->left = rotateLeft(node->left); // LR
        return rotateRight(node); // LL
    }
    if (bf < -1) {
        if (getBalance(node->right) > 0)
            node->right = rotateRight(node->right); // RL
        return rotateLeft(node); // RR
    }
    return node;
}

static Node* insert(Node* node, int key) {
    if (!node) return new Node(key);
    if (key < node->key)
        node->left = insert(node->left, key);
    else if (key > node->key)
        node->right = insert(node->right, key);
    else {
        cout << "Элемент уже существует.\n";
        return node;
    }
    return balance(node);
}

static Node* findMin(Node* node) {
    return node->left ? findMin(node->left) : node;
}

static Node* removeMin(Node* node) {
    if (!node->left) return node->right;
    node->left = removeMin(node->left);
    return balance(node);
}

static Node* remove(Node* node, int key) {
    if (!node) {
        cout << "Элемент не найден.\n";
        return nullptr;
    }
    if (key < node->key)
        node->left = remove(node->left, key);
    else if (key > node->key)
        node->right = remove(node->right, key);
    else {
        Node* left = node->left;
        Node* right = node->right;
        delete node;
        if (!right) return left;
        Node* min = findMin(right);
        min->right = removeMin(right);
        min->left = left;
        return balance(min);
    }
    return balance(node);
}

static void freeTree(Node* node) {
    if (!node) return;
    freeTree(node->left);
    freeTree(node->right);
    delete node;
}

static int inputInt(const string& prompt) {
    int value;
    while (true) {
        cout << prompt;
        if (cin >> value) return value;
        else {
            cout << "Ошибка: введите целое число.\n";
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
        }
    }
}

static void printTreeStructured(Node* root) {
    if (!root) {
        cout << "(пустое дерево)\n";
        return;
    }

    vector<vector<string>> result;
    queue<Node*> q;
    q.push(root);

    while (!q.empty()) {
        int level_size = q.size();
        vector<string> level_values;

        bool has_non_null = false;
        for (int i = 0; i < level_size; ++i) {
            Node* node = q.front();
            q.pop();

            if (node) {
                level_values.push_back(to_string(node->key));
                q.push(node->left);
                q.push(node->right);
                if (node->left || node->right) has_non_null = true;
            }
            else {
                level_values.push_back("");
                q.push(nullptr);
                q.push(nullptr);
            }
        }

        result.push_back(level_values);
        if (!has_non_null) break;
    }

    // Вычисляем отступы для красивого вывода
    int height = result.size();
    for (int level = 0; level < height; ++level) {
        int level_size = result[level].size();
        int indent = (1 << (height - level - 1)) - 1;
        int spacing = (1 << (height - level)) - 1;

        // Отступ слева
        cout << string(indent * 3, ' ');

        for (int i = 0; i < level_size; ++i) {
            cout << setw(3) << result[level][i];
            if (i < level_size - 1) {
                cout << string(spacing * 3, ' ');
            }
        }
        cout << "\n\n";
    }
}

// Меню
static void menu() {
    cout << "\n=== Меню ===\n";
    cout << "1. Добавить элемент\n";
    cout << "2. Удалить элемент\n";
    cout << "3. Показать дерево\n";
    cout << "0. Выход\n";
    cout << "Выберите действие: ";
}

// Главная функция
int main() {
    setlocale(LC_ALL, "Russian");
    Node* root = nullptr;
    int choice;

    do {
        menu();
        choice = inputInt("");

        switch (choice) {
        case 1: {
            int key = inputInt("Введите значение для добавления: ");
            root = insert(root, key);
            break;
        }
        case 2: {
            int key = inputInt("Введите значение для удаления: ");
            root = remove(root, key);
            break;
        }
        case 3:
            cout << "Структура дерева:\n";
            printTreeStructured(root);
            break;
        case 0:
            cout << "Выход...\n";
            break;
        default:
            cout << "Неверный выбор. Повторите.\n";
        }
    } while (choice != 0);

    freeTree(root);
    return 0;
}
