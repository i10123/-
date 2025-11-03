#include <iostream>
#include <iomanip>
#include <queue>
#include <limits>
#include <vector>
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
    Node* T = x->right;

    x->right = y;
    y->left = T;

    updateHeight(y);
    updateHeight(x);

    return x;
}
//     y         x
//    /         / \
//   x    ->   A   y
//  / \           / \
// A   T2        T2  B
static Node* rotateLeft(Node* x) {
    Node* y = x->right;
    Node* T = y->left;

    y->left = x;
    x->right = T;

    updateHeight(x);
    updateHeight(y);

    return y;
}
// x            y
//  \          / \
//   y   ->   x   B
//  / \      / \
// T   B    A   T

static Node* balance(Node* node) {
    updateHeight(node);
    int balance_factor = getBalance(node);

    if (balance_factor > 1) {
        if (getBalance(node->left) < 0)
            node->left = rotateLeft(node->left);
        return rotateRight(node);
    }
      //      z         z
      //     /         /
      //    x    ->   y  ->  y
      //     \       /      / \
      //      y     x      x   z

    if (balance_factor < -1) {
        if (getBalance(node->right) > 0)
            node->right = rotateRight(node->right);
        return rotateLeft(node);
    }
    //   z      z
    //    \      \
    //     x  ->  y   ->   y
    //    /        \      / \
    //   y          x    z   x

    return node;
}

static Node* add(Node* node, int key) {
    if (!node) 
        return new Node(key);

    if (key < node->key)
        node->left = add(node->left, key);
    else if (key > node->key)
        node->right = add(node->right, key);
    else {
        cout << "Элемент уже существует.\n";
        return node;
    }
    return balance(node);
}

static Node* findMin(Node* node) {
    return node->left ? findMin(node->left) : node;
}

static Node* deleteMin(Node* node) {
    if (!node->left) 
        return node->right;
    node->left = deleteMin(node->left);
    return balance(node);
}

static Node* Delete(Node* node, int key) {
    if (!node) {
        cout << "Элемент не найден.\n";
        return nullptr;
    }
    if (key < node->key)
        node->left = Delete(node->left, key);
    else if (key > node->key)
        node->right = Delete(node->right, key);
    else {
        Node* left = node->left;
        Node* right = node->right;
        delete node;

        if (!right) 
            return left;
        Node* min = findMin(right);
        min->right = deleteMin(right);
        min->left = left;
        return balance(min);
    }
    return balance(node);
}

static void clearMemory(Node* node) {
    if (!node) 
        return;
    clearMemory(node->left);
    clearMemory(node->right);
    delete node;
}
static int inputInt(const string& input) {
    int value;
    while (true) {
        cout << input;
        if (cin >> value) 
            return value;
        else {
            cout << "Ошибка: введите целое число.\n";
            cin.clear();
            cin.ignore(numeric_limits<streamsize>::max(), '\n');
        }
    }
}

static void printTree(Node* tree) {
    if (!tree) {
        cout << "(пустое дерево)\n";
        return;
    }

    vector<vector<string>> result;
    queue<Node*> q;
    q.push(tree);

    while (!q.empty()) {
        int level_size = q.size();
        vector<string> level_values;

        bool has_non_null = false;
        for (int i = 0; i < level_size; i++) {
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
        if (!has_non_null) 
            break;
    }

    int height = result.size();
    for (int level = 0; level < height; level++) {
        int level_size = result[level].size();
        int indent = (1 << (height - level - 1)) - 1;
        int spacing = (1 << (height - level)) - 1;

        cout << string(indent * 3, ' ');

        for (int i = 0; i < level_size; i++) {
            cout << setw(3) << result[level][i];
            if (i < level_size - 1) {
                cout << string(spacing * 3, ' ');
            }
        }
        cout << "\n\n";
    }
}

static void menu() {
    cout << "\n=== Меню ===\n";
    cout << "1. Добавить элемент\n";
    cout << "2. Удалить элемент\n";
    cout << "3. Показать дерево\n";
    cout << "0. Выход\n";
    cout << "Выберите действие: ";
}

int main() {
    setlocale(LC_ALL, "Russian");
    Node* tree = nullptr;
    int choice;

    do {
        menu();
        choice = inputInt("");

        switch (choice) {
        case 1: {
            int count = inputInt("Сколько значений вы хотите добавить? ");
            for (int i = 0; i < count; ++i) {
                int key = inputInt("Введите значение №" + to_string(i + 1) + ": ");
                tree = add(tree, key);
            }
            break;
        }
        case 2: {
            int key = inputInt("Введите значение для удаления: ");
            tree = Delete(tree, key);
            break;
        }
        case 3:
            cout << "Структура дерева:\n";
            printTree(tree);
            break;
        case 0:
            cout << "Выход.\n";
            break;
        default:
            cout << "Неверный выбор.\n";
        }
    } while (choice != 0);

    clearMemory(tree);
    return 0;
}
