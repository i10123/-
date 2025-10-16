public class Student implements Comparable<Student> {
    private String fio;
    private String city;
    private String street;
    private String house;
    private String apartment;
    private double distanceToMinsk;

    public Student(String fio, String city, String street, String house, String apartment, double distanceToMinsk) {
        this.fio = fio;
        this.city = city;
        this.street = street;
        this.house = house;
        this.apartment = apartment;
        this.distanceToMinsk = distanceToMinsk;
    }

    public String getFIO() { return fio; }
    public String getCity() { return city; }
    public String getStreet() { return street; }
    public String getHouse() { return house; }
    public String getApartment() { return apartment; }
    public double getDistanceToMinsk() { return distanceToMinsk; }

    public void setFIO(String fio) { this.fio = fio; }
    public void setCity(String city) { this.city = city; }
    public void setStreet(String street) { this.street = street; }
    public void setHouse(String house) { this.house = house; }
    public void setApartment(String apartment) { this.apartment = apartment; }
    public void setDistanceToMinsk(double distance) { this.distanceToMinsk = distance; }

    @Override
    public int compareTo(Student o) {
        return Double.compare(o.distanceToMinsk, this.distanceToMinsk);
    }

    @Override
    public String toString() {
        return fio + " (" + city + ", " + street + " " + house + "-" + apartment + ")           -> " + distanceToMinsk + " км от Минска";
    }
}