import java.io.*;

public class Laptop implements Serializable {
    private String name;
    private double cpuGHz;
    private int ramGB;
    private int hddGB;
    private double price;

    public Laptop(String name, double cpuGHz, int ramGB, int hddGB, double price) {
        this.name = name;
        this.cpuGHz = cpuGHz;
        this.ramGB = ramGB;
        this.hddGB = hddGB;
        this.price = price;
    }
    
    public void setName(String name) {this.name = name;}
    public void setCpuGHz(double cpuGHz) {this.cpuGHz = cpuGHz;}
    public void setRamGB(int ramGB) {this.ramGB = ramGB;}
    public void setHddGB(int hddGB) {this.hddGB = hddGB;}
    public void setPrice(double price) {this.price = price;}

    public String getName() { return name; }
    public double getCpuGHz() { return cpuGHz; }
    public int getRamGB() { return ramGB; }
    public int getHddGB() { return hddGB; }
    public double getPrice() { return price; }

    @Override
    public String toString() {
        return String.format("%-13s | CPU: %.2f GHz | RAM: %3d GB | HDD: %4d GB | %.2f$ ",
                name, cpuGHz, ramGB, hddGB, price);
    }
}