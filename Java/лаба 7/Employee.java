import java.util.Objects;

public abstract class Employee {
    private String name;
    private int age;
    private String department;
    private static int count = 0;

    public Employee(String name, int age, String department) {
        this.name = name;
        this.age = age;
        this.department = department;
        count++;
    }

    // Сеттеры
    public void setName(String name) { 
        this.name = name; 
    }
    public void setAge(int age) { 
        this.age = age; 
    }
    public void setDepartment(String department) { 
        this.department = department; 
    }

    // Геттеры
    public String getName() { 
        return this.name; 
    }
    public int getAge() { 
        return this.age; 
    }
    public String getDepartment() { 
        return this.department; 
    }

    // Абстрактный метод
    public abstract void work();

    // equals с защитой от null
    @Override
    public boolean equals(Object obj) {
        if (this == obj) 
            return true;

        if (!(obj instanceof Employee)) 
            return false;

        Employee other = (Employee) obj;
        
        return age == other.age &&
               Objects.equals(name, other.name) &&
               Objects.equals(department, other.department);
    }

    @Override
    public int hashCode() {
        return Objects.hash(name, age, department);
    }

    // Красивый toString с указанием типа
    @Override
    public String toString() {
        return getClass().getSimpleName() +
               " [Имя: " + name +
               ", Возраст: " + age +
               ", Отдел: " + department + "]";
    }

    // Статический метод
    public static int getCount() {
        return count;
    }
}
