package com.example.backend.controllers;

import com.example.backend.baseClasses.BaseCRUDService;
import com.example.backend.baseClasses.BaseController;
import com.example.backend.data.exceptions.NotAuthException;
import com.example.backend.data.models.Day;
import com.example.backend.data.viewModels.StartEndDate;
import com.example.backend.services.DaysService;
import com.example.backend.services.NoteService;
import com.example.backend.services.UsersService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import javax.servlet.http.HttpServletRequest;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Objects;

@RestController
@RequestMapping("api/days")
public class DaysController extends BaseController<Day, Integer> {

    private final DaysService daysService;
    private final UsersService usersService;

    public DaysController(DaysService daysService, UsersService usersService) {
        super(daysService);

        this.daysService = daysService;
        this.usersService = usersService;
    }

    @GetMapping("/byDates")
    public ResponseEntity<List<Day>> readByDates(@RequestBody StartEndDate startEndDate, HttpServletRequest request)
            throws NotAuthException {
        String token = Arrays.stream(request.getCookies())
                .filter(c -> Objects.equals(c.getName(), "token"))
                .findFirst().get().getValue();
        if (token == null) throw new NotAuthException("Нет токена");

        var days = daysService.read().stream()
                .filter(d -> d.getDate().after(startEndDate.getStart()) && d.getDate().before(startEndDate.getEnd()))
                .toList();

        var currentUser = usersService.readByToken(token);
        if (currentUser == null) return ResponseEntity.notFound().build();
        for (var note : currentUser.getNotes()) {
            days.forEach(day -> {
                if (Objects.equals(note.getDay().getId(), day.getId())) day.setCurrentUserNote(note.getDescription());
            });
        }

        return ResponseEntity.ok(days);
    }
}