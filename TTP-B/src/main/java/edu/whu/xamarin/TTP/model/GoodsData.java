package edu.whu.xamarin.TTP.model;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonIgnore;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import javax.persistence.*;
import java.util.Date;

@Data
@AllArgsConstructor
@NoArgsConstructor
@Builder
@Entity
@Table(name="goodsDataTable")
public class GoodsData {
    public enum TYPE { 工具, 服装, 电子, 其他}

//    @Id
//    @GeneratedValue
//    @JsonIgnore
//    private Long goodsId;

    @Id
    private Long id;

    @Column(nullable = false,length = 32)
    private String name;

    private int price;

    private TYPE type;

    @Column(nullable = false,length = 32)
    public Long userId;

    @JsonFormat(pattern = "yyyy-MM-dd HH:mm", timezone = "GMT+8")
    private Date date;

    @Column(length = 512)
    private String description;

    private String uri;
}
